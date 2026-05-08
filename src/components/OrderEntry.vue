<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { toPng } from 'html-to-image'
import draggable from 'vuedraggable'
import Multiselect from '@vueform/multiselect'
import '@vueform/multiselect/themes/default.css'

const props = defineProps({
  initialData: { type: Object, default: null },
  readonly: { type: Boolean, default: false }
})
const emit = defineEmits(['saved'])

const formData = ref({
  vendor: '',
  orderDate: '',
  fabricType: '',
  orderNo: '',
  maleStyleNo: '',
  femaleStyleNo: '',
  detailStyleNo: '',
  deliveryDate: '',
  cuttingDate: '',
  threadCode: '',
  composition: '',
  notes: ''
})

const fabricOptions = [
  '1-M+T高爾布',
  '2-C+T高爾布',
  '3-超新吸排高爾布',
  '4-細針雙面排汗布',
  '5-壓花排汗組織布',
  '6-階梯排汗組織布',
  '7-C+T組織布',
  '8-M+T組織布',
  '9-細針橫條組織布'
]

const compositionOptions = [
  'POLYESTER 100%',
  'POLYESTER 70% / COTTON 30%'
]

// Image Load Error Handling
const maleImgValid = ref(true)
const femaleImgValid = ref(true)
const detailImgValid = ref(true)

const getMaleImgUrl = computed(() => {
  const val = formData.value.maleStyleNo?.trim()
  return val ? `/styles/${val}.jpg` : null
})
const getFemaleImgUrl = computed(() => {
  const val = formData.value.femaleStyleNo?.trim()
  return val ? `/styles/${val}.jpg` : null
})
const getDetailImgUrl = computed(() => {
  const val = formData.value.detailStyleNo?.trim()
  return val ? `/styles/${val}.jpg` : null
})

// Dynamically load image names from public/styles for the datalist
const imagePaths = Object.keys(import.meta.glob('/public/styles/*.{jpg,jpeg,png,JPG,JPEG,PNG}'))
const styleOptions = ref(imagePaths.map(path => {
  const filename = path.split('/').pop()
  return filename.substring(0, filename.lastIndexOf('.'))
}))

const handleImgError = (type) => {
  if (type === 'male') maleImgValid.value = false
  else if (type === 'female') femaleImgValid.value = false
  else detailImgValid.value = false
}

const resetImgStatus = (type) => {
  if (type === 'male') maleImgValid.value = true
  else if (type === 'female') femaleImgValid.value = true
  else detailImgValid.value = true
}

// Watch inputs to reset error status
watch(() => formData.value.maleStyleNo, () => resetImgStatus('male'))
watch(() => formData.value.femaleStyleNo, () => resetImgStatus('female'))
watch(() => formData.value.detailStyleNo, () => resetImgStatus('detail'))

const sizes = ref(['S', 'M', 'L', 'XL', '2L', '3L', '5L'])
const newSizeLabel = ref('')

const addSize = () => {
  const label = newSizeLabel.value.trim().toUpperCase()
  if (label && !sizes.value.includes(label)) {
    sizes.value.push(label)
    newSizeLabel.value = ''
    syncMatrixKeys()
  }
}

const removeSize = (s) => {
  sizes.value = sizes.value.filter(item => item !== s)
  syncMatrixKeys()
}

const matrix = ref({
  order: { male: {}, female: {} },
  cut: { male: {}, female: {} }
})

const syncMatrixKeys = () => {
  const cats = ['order', 'cut']; const gens = ['male', 'female']
  cats.forEach(c => gens.forEach(g => sizes.value.forEach(s => {
    if (!(s in matrix.value[c][g])) matrix.value[c][g][s] = null
  })))
}

// Order No Separation Logic (Year - ID)
const formOrderYear = computed({
  get: () => {
    const defaultYear = new Date().getFullYear().toString()
    if (!formData.value.orderNo) return defaultYear
    const parts = formData.value.orderNo.split('-')
    if (parts.length > 1) return parts[0]
    return defaultYear
  },
  set: (val) => {
    const id = formOrderId.value
    formData.value.orderNo = `${val}-${id}`
  }
})

const formOrderId = computed({
  get: () => {
    if (!formData.value.orderNo) return ''
    const parts = formData.value.orderNo.split('-')
    return parts.length > 1 ? parts.slice(1).join('-') : formData.value.orderNo
  },
  set: (val) => {
    const year = formOrderYear.value
    formData.value.orderNo = val ? `${year}-${val}` : ''
  }
})

// Population Logic
const populateData = (data) => {
  if (!data || !data.formData) {
    console.error('Invalid data structure passed to OrderEntry', data)
    return
  }
  
  // 1. 填充表單基礎欄位
  Object.keys(formData.value).forEach(key => {
    if (data.formData[key] !== undefined) {
      formData.value[key] = data.formData[key]
    }
  })
  
  // 2. 填充尺寸列表
  if (data.sizes && Array.isArray(data.sizes)) {
    sizes.value = JSON.parse(JSON.stringify(data.sizes))
  }
  
  // 3. 填充數量矩陣
  if (data.matrix) {
    matrix.value = JSON.parse(JSON.stringify(data.matrix))
  }
  
  // 4. 同步與校檢
  syncMatrixKeys()
  resetImgStatus('male')
  resetImgStatus('female')
  resetImgStatus('detail')
}

onMounted(() => {
  if (props.initialData) {
    populateData(props.initialData)
  } else {
    // 建立新表單：清空所有欄位
    formData.value = {
      vendor: '',
      orderDate: '',
      fabricType: '',
      orderNo: '', 
      maleStyleNo: '',
      femaleStyleNo: '',
      detailStyleNo: '',
      deliveryDate: '',
      cuttingDate: '',
      threadCode: '',
      composition: '',
      notes: ''
    }
    // 重置矩陣
    matrix.value = {
      order: { male: {}, female: {} },
      cut: { male: {}, female: {} }
    }
    syncMatrixKeys()
  }
})

// Watch for prop changes (e.g. searching different orders)
watch(() => props.initialData, (newVal) => {
  if (newVal) populateData(newVal)
}, { deep: true, immediate: true })

const getRowTotal = (row) => sizes.value.reduce((a, s) => a + (Number(row[s]) || 0), 0)
const maleOrderTotal = computed(() => getRowTotal(matrix.value.order.male))
const femaleOrderTotal = computed(() => getRowTotal(matrix.value.order.female))
const maleCutTotal = computed(() => getRowTotal(matrix.value.cut.male))
const femaleCutTotal = computed(() => getRowTotal(matrix.value.cut.female))

const productionFormRef = ref(null)
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:8081') + '/api/ProductionOrder'

// 轉換資料格式以符合後端 Model
const preparePayload = () => {
  const items = []
  
  // 遍歷矩陣並轉換為扁平列表
  const categories = ['order', 'cut']
  const genders = ['male', 'female']
  
  categories.forEach(cat => {
    genders.forEach(gen => {
      sizes.value.forEach(s => {
        const qty = matrix.value[cat][gen][s]
        if (qty !== null && qty !== undefined && qty !== '') {
          items.push({
            category: cat,
            gender: gen,
            size: s,
            quantity: Number(qty)
          })
        }
      })
    })
  })

  return {
    ...formData.value,
    id: props.initialData?.id || 0,
    sizesJson: JSON.stringify(sizes.value),
    items: items
  }
}

const saveToDatabase = async () => {
  // 1. 必填欄位驗證
  const requiredFields = [
    { label: '廠商', value: formData.value.vendor },
    { label: '訂購日期', value: formData.value.orderDate }
  ]

  const missingFields = requiredFields.filter(f => !f.value || f.value.trim() === '')
  
  if (missingFields.length > 0) {
    const missingLabels = missingFields.map(f => f.label).join('、')
    alert(`❌ 儲存失敗：請確實填寫必填欄位 (${missingLabels})！`)
    return // 終止儲存流程
  }

  // 2. 準備 Payload 並打 API
  const payload = preparePayload()
  const isEdit = !!props.initialData?.id
  const method = isEdit ? 'PUT' : 'POST'
  const url = isEdit ? `${API_BASE_URL}/${props.initialData.id}` : API_BASE_URL

  try {
    const resp = await fetch(url, {
      method: method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })

    if (!resp.ok) {
      const err = await resp.json()
      throw new Error(err.message || (isEdit ? '更新失敗' : '儲存失敗'))
    }

    alert(isEdit ? '✅ 生產單更新成功！' : '🚀 生產單已成功儲存至資料庫！')
    
    // 成功後發送事件，讓父元件執行跳轉
    emit('saved')
  } catch (error) {
    alert(`❌ 錯誤: ${error.message}`)
  }
}

const saveAsImage = () => {
  if (!productionFormRef.value) return
  toPng(productionFormRef.value, { backgroundColor: '#fff', cacheBust: true })
    .then((u) => { const l = document.createElement('a'); l.download = `生產單_${formData.value.orderNo}.png`; l.href = u; l.click() })
    .catch(() => alert('轉換圖片時發生錯誤'))
}
const printDocument = () => window.print()
</script>

<template>
  <div class="production-wrapper">
    <!-- Size Management (UI Only) - Hidden if Readonly -->
    <div v-if="!readonly" class="size-controls no-print mb-4 shadow-sm">
      <h3 class="text-left text-blue mb-3">尺寸管理與排序</h3>
      <div class="add-size-container mb-3">
        <input type="text" v-model="newSizeLabel" placeholder="輸入尺寸" class="form-input size-input-tool" @keyup.enter="addSize" />
        <button class="btn btn-primary btn-sm add-size-btn" @click="addSize">新增</button>
      </div>
      <draggable v-model="sizes" item-key="index" class="size-drag-list" handle=".drag-handle">
        <template #item="{ element, index }">
          <div class="size-token-draggable">
            <span class="drag-handle">
              <span class="index-badge-visible">{{ index + 1 }}</span>
            </span>
            <span class="size-text">{{ element }}</span>
            <button @click="removeSize(element)" class="size-close-icon">✕</button>
          </div>
        </template>
      </draggable>
    </div>

    <!-- The actual Production Order -->
    <div class="production-form document-style fade-in" ref="productionFormRef">
      <h1 class="document-title">生 產 單</h1>

      <!-- Main Upper Layout: Left Sidebar (Inputs) | Right Content (Display) -->
      <!-- Option A Layout: Top Fields (2-column), Middle Notes, Bottom Images -->
      <div class="main-doc-layout-a">
        <!-- Top: 2-Column Form Fields -->
        <div class="fields-two-column-area mb-3">
          <div class="field-item"><label>廠商 <span class="text-red">*</span>：</label><input type="text" v-model="formData.vendor" class="border-bottom" :disabled="readonly" required /></div>
          <div class="field-item">
            <label>訂購日期 <span class="text-red">*</span>：</label>
            <div class="date-print-wrapper">
              <input type="date" v-model="formData.orderDate" class="border-bottom date-input no-print" :disabled="readonly" required />
              <span class="print-only date-print-value">{{ formData.orderDate || '' }}</span>
            </div>
          </div>
          <div class="field-item">
            <label>單號 <span class="text-red">*</span>：</label>
            <div class="split-input-wrapper">
              <input type="text" v-model="formOrderYear" class="border-bottom bold year-input no-print" :disabled="readonly" />
              <span class="print-only bold year-input border-bottom print-inline">{{ formOrderYear }}</span>
              <span class="hyphen">-</span>
              <input type="text" v-model="formOrderId" class="border-bottom bold id-input no-print" placeholder="輸入單號..." :disabled="readonly" required />
              <span class="print-only bold id-input border-bottom print-inline">{{ formOrderId }}</span>
            </div>
          </div>
          <div class="field-item">
            <label>布種：</label>
            <Multiselect
              v-model="formData.fabricType"
              :options="fabricOptions"
              :searchable="true"
              :create-option="true"
              placeholder="選擇或輸入布種..."
              :disabled="readonly"
              class="custom-multiselect"
            />
          </div>
          <div class="field-item">
            <label>男款型號：</label>
            <Multiselect
              v-model="formData.maleStyleNo"
              :options="styleOptions"
              :searchable="true"
              :create-option="true"
              placeholder="輸入型號..."
              :disabled="readonly"
              class="custom-multiselect bold-multiselect"
            />
          </div>
          <div class="field-item">
            <label>女款型號：</label>
            <Multiselect
              v-model="formData.femaleStyleNo"
              :options="styleOptions"
              :searchable="true"
              :create-option="true"
              placeholder="輸入型號..."
              :disabled="readonly"
              class="custom-multiselect bold-multiselect"
            />
          </div>
          <div class="field-item">
            <label>交貨日期：</label>
            <div class="date-print-wrapper">
              <input type="date" v-model="formData.deliveryDate" class="border-bottom date-input no-print" :disabled="readonly" />
              <span class="print-only date-print-value">{{ formData.deliveryDate || '' }}</span>
            </div>
          </div>
          <div class="field-item">
            <label>裁剪日期：</label>
            <div class="date-print-wrapper">
              <input type="date" v-model="formData.cuttingDate" class="border-bottom date-input no-print" :disabled="readonly" />
              <span class="print-only date-print-value">{{ formData.cuttingDate || '' }}</span>
            </div>
          </div>
          <div class="field-item"><label>線號：</label><input type="text" v-model="formData.threadCode" class="border-bottom" :disabled="readonly" /></div>
          <div class="field-item">
            <label>細節圖型號：</label>
            <Multiselect
              v-model="formData.detailStyleNo"
              :options="styleOptions"
              :searchable="true"
              :create-option="true"
              placeholder="輸入細節型號..."
              :disabled="readonly"
              class="custom-multiselect bold-multiselect"
            />
          </div>
          <div class="field-item span-2">
            <label>成分/標示：</label>
            <Multiselect
              v-model="formData.composition"
              :options="compositionOptions"
              :searchable="true"
              :create-option="true"
              placeholder="選擇或輸入成分..."
              :disabled="readonly"
              class="custom-multiselect"
            />
          </div>
        </div>

        <!-- Middle: Notes -->
        <div class="notes-box-doc notes-horizontal-wide mb-3">
          <div class="notes-label-internal text-red">注意事項：</div>
          <textarea v-model="formData.notes" class="notes-textarea-internal" placeholder="補充說明..." :disabled="readonly"></textarea>
        </div>

        <!-- Bottom: 3 Images Side-by-Side -->
        <div class="photos-three-row mb-3">
          <div class="photo-box-doc">
            <div v-if="!getMaleImgUrl || !maleImgValid" class="photo-placeholder"><span>男款型號<br>{{ formData.maleStyleNo || '未填寫' }}</span></div>
            <img v-else :src="getMaleImgUrl" class="doc-img-fill" @error="handleImgError('male')" />
          </div>
          <div class="photo-box-doc">
            <div v-if="!getFemaleImgUrl || !femaleImgValid" class="photo-placeholder"><span>女款型號<br>{{ formData.femaleStyleNo || '未填寫' }}</span></div>
            <img v-else :src="getFemaleImgUrl" class="doc-img-fill" @error="handleImgError('female')" />
          </div>
          <div class="photo-box-doc">
            <div v-if="!getDetailImgUrl || !detailImgValid" class="photo-placeholder"><span>細節圖型號<br>{{ formData.detailStyleNo || '未填寫' }}</span></div>
            <img v-else :src="getDetailImgUrl" class="doc-img-fill" @error="handleImgError('detail')" />
          </div>
        </div>
      </div>

      <!-- Large Quantity Table (Bottom) -->
      <div class="quantity-table-full">
        <table class="doc-table-large">
          <thead>
            <tr>
              <th colspan="2" class="table-label-main">尺寸</th>
              <th v-for="s in sizes" :key="s" class="size-header-large">{{ s }}</th>
              <th class="total-header-large">總計</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td rowspan="2" class="category-cell-large">訂購量</td>
              <td class="gender-cell-large">男</td>
              <td v-for="s in sizes" :key="s">
                <input type="number" v-model="matrix.order.male[s]" class="table-qty-input-large" :disabled="readonly" />
              </td>
              <td class="total-qty-cell-large">{{ maleOrderTotal }}</td>
            </tr>
            <tr>
              <td class="gender-cell-large text-red">女</td>
              <td v-for="s in sizes" :key="s">
                <input type="number" v-model="matrix.order.female[s]" class="table-qty-input-large" :disabled="readonly" />
              </td>
              <td class="total-qty-cell-large">{{ femaleOrderTotal }}</td>
            </tr>
            <tr>
              <td rowspan="2" class="category-cell-large text-red">裁剪量</td>
              <td class="gender-cell-large">男</td>
              <td v-for="s in sizes" :key="s">
                <input type="number" v-model="matrix.cut.male[s]" class="table-qty-input-large" :disabled="readonly" />
              </td>
              <td class="total-qty-cell-large">{{ maleCutTotal }}</td>
            </tr>
            <tr>
              <td class="gender-cell-large text-red">女</td>
              <td v-for="s in sizes" :key="s">
                <input type="number" v-model="matrix.cut.female[s]" class="table-qty-input-large" :disabled="readonly" />
              </td>
              <td class="total-qty-cell-large">{{ femaleCutTotal }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Actions -->
    <div class="doc-actions no-print mt-4 sticky-actions">
      <button v-if="!readonly" class="btn btn-primary main-save-btn db-save-btn" @click="saveToDatabase">
        {{ props.initialData?.id ? '💾 更新資料庫' : '🚀 儲存至資料庫' }}
      </button>
      <button class="btn btn-outline" @click="saveAsImage">🖼️ 儲存生產單圖片</button>
      <button class="btn btn-outline" @click="printDocument">🖨️ 列印生產單</button>
    </div>
  </div>
</template>

<style scoped>
.production-wrapper { padding-bottom: 2rem; }
.production-form { background: white; padding: 1.5rem; border: 2px solid #000; color: #000; width: 100%; max-width: 100%; border-radius: 2px; font-family: "標楷體", "DFKai-SB", "BiauKai", serif; display: flex; flex-direction: column; min-height: 297mm; }
.document-title { font-size: 2.2rem; font-weight: 900; letter-spacing: 0.8rem; margin-bottom: 1.5rem; border-bottom: 2.5px solid #000; padding-bottom: 0.5rem; }

/* Option A Layout Styles */
.main-doc-layout-a { display: flex; flex-direction: column; width: 100%; margin-bottom: 1rem; }

/* Top: 2-Column Fields */
.fields-two-column-area { display: grid; grid-template-columns: 1fr 1fr; column-gap: 2rem; row-gap: 1.25rem; text-align: left; }
.field-item { display: flex; align-items: center; justify-content: space-between; margin-bottom: 0; }
.field-item label { font-weight: 800; white-space: nowrap; font-size: 1.1rem; flex: 0 0 110px; }
.span-2 { grid-column: span 2; }
.border-bottom { border: none; border-bottom: 1.5px solid #000; padding: 4px 4px 8px 4px; width: 100%; font-size: 1.25rem; font-weight: 500; background: transparent; }
.border-red { border-bottom-color: #dc2626; }
.bold { font-weight: 900; font-size: 1.3rem; }

/* Custom Multiselect styles to match border-bottom */
.custom-multiselect {
  --ms-bg: transparent;
  --ms-border-color: transparent;
  --ms-border-width: 0 0 1.5px 0;
  --ms-border-color-active: #000;
  --ms-radius: 0;
  --ms-ring-width: 0;
  --ms-font-size: 1.25rem;
  --ms-py: 4px;
  --ms-px: 4px;
  --ms-option-font-size: 1.1rem;
  --ms-caret-color: #000;
  --ms-clear-color: #000;
  --ms-dropdown-border-color: #ccc;
  --ms-dropdown-radius: 4px;
  flex: 1;
  min-height: auto;
  border-bottom: 1.5px solid #000 !important;
  padding-bottom: 4px;
}
.custom-multiselect .multiselect-single {
  font-weight: 500;
}
.bold-multiselect {
  --ms-font-size: 1.3rem;
}
.bold-multiselect .multiselect-single {
  font-weight: 900;
}
.custom-multiselect.is-active {
  box-shadow: none;
  border-bottom-color: var(--accent-color, #3b82f6) !important;
}

/* Multiselect Icon Spacing and Hover */
.custom-multiselect :deep(.multiselect-clear) { margin-right: 16px; border-radius: 6px; transition: all 0.2s; padding: 4px; }
.custom-multiselect :deep(.multiselect-caret) { border-radius: 6px; transition: all 0.2s; padding: 4px; }
.custom-multiselect :deep(.multiselect-clear:hover),
.custom-multiselect :deep(.multiselect-caret:hover) { background-color: #e2e8f0; color: #2563eb; transform: scale(1.15); }

/* Date Picker Icon Spacing and Hover */
input[type="date"]::-webkit-calendar-picker-indicator { cursor: pointer; padding: 6px; border-radius: 6px; margin-left: 12px; transition: all 0.2s; }
input[type="date"]::-webkit-calendar-picker-indicator:hover { background-color: #e2e8f0; transform: scale(1.15); }

.split-input-wrapper { display: flex; align-items: center; gap: 0.5rem; width: 100%; }
.year-input { flex: 0 0 75px; text-align: center; }
.id-input { flex: 1; }
.hyphen { font-size: 1.5rem; font-weight: 900; }
.print-inline { display: inline-block; text-align: center; }
.date-input { font-family: inherit; } /* 使日期選擇器字體一致 */

.date-print-wrapper { position: relative; width: 100%; display: flex; align-items: center; }
.print-only { display: none; }
@media print {
  .print-only { display: inline-block; }
  .no-print { display: none !important; }
}
.date-print-value { width: 100%; border-bottom: 1.5px solid #000; padding: 4px; font-size: 1.25rem; min-height: 1.5rem; text-align: left; }

/* Middle: Notes */
.notes-horizontal-wide { margin-top: 1.5rem; margin-bottom: 1.5rem; height: 180px; width: 100%; display: flex; flex-direction: column; border: 2px solid #000; padding: 0.5rem 1rem; text-align: left; }
.notes-label-internal { font-size: 1.1rem; font-weight: 800; margin-bottom: 0.25rem; border-bottom: 1px dashed #ccc; }
.notes-textarea-internal { width: 100%; flex: 1; border: none; padding: 0.5rem 0; font-size: 1.1rem; line-height: 1.6; resize: none; font-family: inherit; background: transparent; outline: none; overflow: hidden; }

/* Bottom: 3 Images Row */
.photos-three-row { display: flex; gap: 1rem; width: 100%; min-height: 250px; }
.photo-box-doc { border: 2px solid #000; flex: 1; background-color: #fdfdfd; display: flex; overflow: hidden; align-items: center; justify-content: center; position: relative; aspect-ratio: 3 / 4; }
.photo-placeholder { color: #64748b; text-align: center; font-size: 0.85rem; font-weight: 600; line-height: 1.4; padding: 1rem; }
.doc-img-fill { width: 100%; height: 100%; object-fit: contain; }

/* Quantity Table */
.quantity-table-full { margin-top: 1rem; width: 100%; }
.doc-table-large { border: 2.5px solid #000; width: 100%; border-collapse: collapse; table-layout: fixed; }
.doc-table-large th { background-color: #f1f5f9; border: 1.5px solid #000; padding: 8px 2px; font-weight: 900; font-size: 0.95rem; }
.doc-table-large td { border: 1.5px solid #333; padding: 2px; height: 44px; }

.category-cell-large { width: 66px; font-weight: 900; font-size: 1.05rem; }
.gender-cell-large { width: 36px; font-weight: 900; font-size: 1rem; }
.table-qty-input-large { width: 100%; border: none; padding: 6px 0; text-align: center; font-size: 1.2rem; font-weight: 800; background: transparent; }
.total-qty-cell-large { background-color: #f8fafc; font-weight: 900; font-size: 1.2rem; text-align: center; }
.size-close-icon { width: 1.25rem; height: 1.25rem; background: #f1f5f9; border: 1px solid #cbd5e1; border-radius: 50%; color: #64748b; display: flex; align-items: center; justify-content: center; font-size: 0.65rem; font-weight: 700; cursor: pointer; transition: all 0.15s; padding: 0; line-height: 1; margin-left: 0.25rem; }
.size-close-icon:hover { background: #fee2e2; color: #dc2626; border-color: #f87171; box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1); }

/* Printing Optimization */
@media print {
  @page { size: A4 portrait; margin: 8mm; }
  
  .production-wrapper { padding-bottom: 0 !important; }
  
  .production-form { 
    padding: 5px !important; 
    border: none !important; 
    display: flex !important;
    flex-direction: column !important;
    height: 280mm !important;
    max-height: 280mm !important;
    overflow: hidden !important;
  }
  
  .document-title { font-size: 1.8rem !important; margin-bottom: 5mm !important; padding-bottom: 0 !important; letter-spacing: 0.5rem !important; }
  
  .main-doc-layout-a { margin-bottom: 2mm !important; flex: 1 !important; display: flex !important; flex-direction: column !important; }
  
  /* 兩欄表單間距放寬與字體放大 */
  .fields-two-column-area { row-gap: 0.5rem !important; column-gap: 5mm !important; margin-bottom: 4mm !important; }
  .field-item label { font-size: 1.25rem !important; flex: 0 0 100px !important; }
  .border-bottom { font-size: 1.25rem !important; padding: 0 0 2px 0 !important; border-bottom-width: 1px !important; min-height: 26px !important; line-height: 1.3 !important; }
  .custom-multiselect { padding-bottom: 2px !important; --ms-py: 2px !important; --ms-font-size: 1.25rem !important; min-height: 26px !important; }
  .custom-multiselect :deep(.multiselect-single) { font-size: 1.25rem !important; line-height: 1.3 !important; }
  
  /* 注意事項自動彈性撐滿剩餘空間 */
  .notes-horizontal-wide { flex: 1 !important; height: auto !important; padding: 5px !important; margin-top: 2mm !important; margin-bottom: 2mm !important; }
  .notes-textarea-internal { font-size: 1.2rem !important; padding: 0 !important; }
  
  /* 圖片區塊 */
  .photos-three-row { gap: 5mm !important; margin-bottom: 2mm !important; }
  .photo-box-doc { height: auto !important; aspect-ratio: 3 / 4 !important; border-width: 1px !important; }
  
  /* 表格高度與框線縮減 */
  .quantity-table-full { margin-top: 2mm !important; padding-top: 0 !important; }
  .doc-table-large { border-width: 1.5px !important; }
  .doc-table-large th { padding: 4px 2px !important; }
  .doc-table-large td { height: 26px !important; padding: 0 !important; border-width: 1px !important; }
  .table-qty-input-large { font-size: 1rem !important; padding: 0 !important; height: 26px !important; }
  .total-qty-cell-large { font-size: 1.1rem !important; height: 26px !important; }
  
  /* 隱藏不需要的互動元素 */
  .no-print, .sticky-actions, .size-close-icon { display: none !important; }
  
  /* 隱藏佔位符與圖示 */
  ::placeholder { color: transparent !important; }
  ::-webkit-input-placeholder { color: transparent !important; }
  :-moz-placeholder { color: transparent !important; }
  ::-moz-placeholder { color: transparent !important; }
  :-ms-input-placeholder { color: transparent !important; }
  
  :deep(.multiselect-placeholder) { display: none !important; }
  :deep(.multiselect-clear), :deep(.multiselect-caret) { display: none !important; }
  input[type="date"]::-webkit-calendar-picker-indicator { display: none !important; }
}

.size-controls { padding: 1.5rem; background: #fff; border: 1px solid #e2e8f0; border-radius: 8px; margin-bottom: 2rem; }
.size-drag-list { display: flex; flex-wrap: wrap; gap: 1rem; margin-top: 1rem; }
.size-token-draggable { display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; background: #fff; border: 2px solid var(--accent-color); border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }
.drag-handle { display: flex; align-items: center; justify-content: center; position: relative; cursor: grab; user-select: none; margin-right: 8px; min-width: 1.5rem; }
.drag-handle:active { cursor: grabbing; }
.index-badge-visible {
  background: #e2e8f0;
  color: #1e293b;
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  font-size: 0.85rem;
  font-weight: 800;
  display: flex;
  align-items: center;
  justify-content: center;
}
.size-text { font-weight: 900; color: var(--accent-color); font-size: 1.15rem; }
.doc-actions { display: flex; justify-content: center; gap: 1rem; padding: 1rem; }
.db-save-btn { background-color: var(--accent-color, #059669); border-color: var(--accent-color, #059669); min-width: 160px; color: white; }
.db-save-btn:hover { filter: brightness(0.9); }

/* 新增尺寸的左右間隔容器 */
.add-size-container {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}
.size-input-tool {
  flex: 1;
  max-width: 250px;
  padding: 0.5rem 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 1.05rem;
  outline: none;
  transition: border-color 0.2s;
}
.size-input-tool:focus {
  border-color: var(--accent-color, #3b82f6);
}
.add-size-btn {
  padding: 0.5rem 1.25rem;
  font-weight: 700;
  border-radius: 6px;
  background-color: var(--accent-color, #0ea5e9);
  color: white;
  border: none;
  cursor: pointer;
  transition: filter 0.2s;
}
.add-size-btn:hover {
  filter: brightness(0.9);
}
</style>
