<script setup>
import { ref, computed } from 'vue'
import OrderEntry from './OrderEntry.vue'

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:8081') + '/api/ProductionOrder'
const allOrders = ref([])
const isLoading = ref(false)
const vendorQuery = ref('')
const styleQuery = ref('')
const selectedOrder = ref(null)
const editMode = ref(false)

const mapApiToFrontend = (apiOrder) => {
  const matrix = {
    order: { male: {}, female: {} },
    cut: { male: {}, female: {} }
  }
  
  const sizes = JSON.parse(apiOrder.sizesJson || '[]')
  
  // 初始化矩陣
  sizes.forEach(s => {
    matrix.order.male[s] = null
    matrix.order.female[s] = null
    matrix.cut.male[s] = null
    matrix.cut.female[s] = null
  })

  // 填入數量
  apiOrder.items?.forEach(item => {
    if (matrix[item.category] && matrix[item.category][item.gender]) {
      matrix[item.category][item.gender][item.size] = item.quantity
    }
  })

  return {
    id: apiOrder.id,
    formData: {
      vendor: apiOrder.vendor,
      orderDate: apiOrder.orderDate,
      fabricType: apiOrder.fabricType,
      orderNo: apiOrder.orderNo,
      maleStyleNo: apiOrder.maleStyleNo,
      femaleStyleNo: apiOrder.femaleStyleNo,
      detailStyleNo: apiOrder.detailStyleNo,
      deliveryDate: apiOrder.deliveryDate,
      cuttingDate: apiOrder.cuttingDate,
      threadCode: apiOrder.threadCode,
      composition: apiOrder.composition,
      notes: apiOrder.notes
    },
    sizes: sizes,
    matrix: matrix
  }
}

const fetchOrders = async () => {
  isLoading.value = true
  try {
    const resp = await fetch(API_BASE_URL)
    const data = await resp.json()
    allOrders.value = data.map(mapApiToFrontend)
  } catch (error) {
    console.error('Fetch error:', error)
  } finally {
    isLoading.value = false
  }
}

import { onMounted } from 'vue'
onMounted(fetchOrders)

const filteredOrders = computed(() => {
  if (!allOrders.value) return []
  return allOrders.value.filter(o => {
    const vendor = o.formData?.vendor || ''
    const maleStyle = o.formData?.maleStyleNo || ''
    const femaleStyle = o.formData?.femaleStyleNo || ''
    
    const vMatch = vendor.toLowerCase().includes(vendorQuery.value.toLowerCase())
    const sMatch = (maleStyle + femaleStyle).toLowerCase().includes(styleQuery.value.toLowerCase())
    return vMatch && sMatch
  })
})

const selectOrder = (order, edit = false) => {
  selectedOrder.value = order
  editMode.value = edit
}

const deleteOrder = async (order) => {
  if (!confirm(`確定要刪除單號 ${order.formData.orderNo} 嗎？`)) return
  
  try {
    const resp = await fetch(`${API_BASE_URL}/${order.id}`, { method: 'DELETE' })
    if (resp.ok) {
      alert('已成功刪除')
      fetchOrders()
    } else {
      alert('刪除失敗')
    }
  } catch (error) {
    alert('偵測到錯誤')
  }
}

const goBack = () => {
  selectedOrder.value = null
  editMode.value = false
  fetchOrders() // 返回時重新整理列表
}
</script>

<template>
  <div class="search-container">
    <transition name="fade" mode="out-in">
      <!-- 詳情查看模式 (唯讀) -->
      <div v-if="selectedOrder" class="detail-view">
        <div class="detail-header no-print">
          <button class="btn btn-outline btn-back" @click="goBack">⬅️ 返回搜尋列表</button>
          <div class="header-info">
            <span v-if="!editMode" class="readonly-badge">唯讀模式</span>
            <span v-else class="edit-badge">編輯模式</span>
            <span class="view-status">單號：#{{ selectedOrder.formData.orderNo }}</span>
          </div>
        </div>
        <OrderEntry :initialData="selectedOrder" :readonly="!editMode" @saved="goBack" />
      </div>

      <!-- 搜尋列表模式 -->
      <div v-else class="list-view">
        <div class="search-header">
          <h2 class="search-title">🔎 歷程查詢</h2>
          <div class="dual-search-grid">
            <div class="search-field">
              <label>廠商名稱</label>
              <input type="text" v-model="vendorQuery" placeholder="搜尋廠商..." class="search-input" />
            </div>
            <div class="search-field">
              <label>型號款式</label>
              <input type="text" v-model="styleQuery" placeholder="搜尋型號..." class="search-input" />
            </div>
          </div>
        </div>

        <div class="search-results">
          <div v-if="filteredOrders.length === 0" class="no-results">
            找不到符合條件的生產單。
          </div>
          <div 
            v-for="order in filteredOrders" 
            :key="order.id" 
            class="order-card"
          >
            <div class="card-main">
              <div class="order-id">#{{ order.formData.orderNo }}</div>
              <div class="order-vendor">{{ order.formData.vendor }}</div>
            </div>
            <div class="card-details">
              <span>型號: {{ order.formData.maleStyleNo }}/{{ order.formData.femaleStyleNo }}</span>
              <span>布種: {{ order.formData.fabricType }}</span>
              <span>日期: {{ order.formData.orderDate }}</span>
            </div>
            <div class="card-footer-actions">
              <button class="btn-card btn-view" @click="selectOrder(order, false)">👁️ 檢視</button>
              <button class="btn-card btn-edit" @click="selectOrder(order, true)">✏️ 編輯</button>
              <button class="btn-card btn-delete" @click.stop="deleteOrder(order)">🗑️ 刪除</button>
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<style scoped>
.search-container { min-height: 80vh; }

/* 詳情頁頂部 */
.detail-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 1.5rem; background: #f8fafc; padding: 1rem 1.5rem; border-radius: 8px; border: 1px solid #e2e8f0; }
.btn-back { font-weight: 700; padding: 0.6rem 1.2rem; }
.header-info { display: flex; align-items: center; gap: 1rem; }
.readonly-badge { background: #64748b; color: white; padding: 0.25rem 0.6rem; border-radius: 4px; font-size: 0.8rem; font-weight: 800; }
.edit-badge { background: #2563eb; color: white; padding: 0.25rem 0.6rem; border-radius: 4px; font-size: 0.8rem; font-weight: 800; }
.view-status { font-weight: 800; color: #1e293b; font-size: 1.1rem; }

/* 雙欄搜尋 */
.search-header { background: white; padding: 2rem; border-radius: 12px; border: 1px solid #e2e8f0; margin-bottom: 2rem; text-align: left; }
.search-title { margin-bottom: 1.5rem; color: #1e293b; font-weight: 900; }
.dual-search-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; }
.search-field label { display: block; font-size: 0.85rem; font-weight: 800; color: #64748b; margin-bottom: 0.5rem; }

.search-input { width: 100%; padding: 0.85rem 1.2rem; font-size: 1rem; border: 2px solid #e2e8f0; border-radius: 10px; transition: all 0.3s; background: #fcfcfc; }
.search-input:focus { outline: none; border-color: #2563eb; background: white; box-shadow: 0 0 0 4px rgba(37, 99, 235, 0.1); }

/* 卡片清單 */
.search-results { display: grid; gap: 1rem; }
.order-card { background: white; padding: 1.5rem; border-radius: 12px; border: 1px solid #e2e8f0; cursor: pointer; transition: all 0.2s; display: flex; flex-direction: column; gap: 1rem; text-align: left; position: relative; }
.order-card:hover { border-color: #2563eb; transform: translateY(-3px); box-shadow: 0 12px 20px -5px rgb(0 0 0 / 0.1); }

.card-main { display: flex; justify-content: space-between; align-items: center; }
.order-id { font-size: 1.4rem; font-weight: 900; color: #1e293b; }
.order-vendor { font-size: 1.1rem; font-weight: 700; color: #2563eb; }

.card-details { display: flex; gap: 1.5rem; color: #64748b; font-size: 0.9rem; font-weight: 600; }
.card-footer-actions { display: flex; gap: 0.5rem; justify-content: flex-end; margin-top: 0.5rem; }
.btn-card { padding: 0.4rem 0.8rem; border-radius: 6px; font-size: 0.85rem; font-weight: 700; cursor: pointer; transition: all 0.2s; border: 1px solid #e2e8f0; background: white; }
.btn-view:hover { background: #f1f5f9; color: #1e293b; border-color: #cbd5e1; }
.btn-edit:hover { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.btn-delete:hover { background: #fef2f2; color: #dc2626; border-color: #fecaca; }

.no-results { padding: 5rem; text-align: center; color: #94a3b8; font-size: 1.2rem; background: #fff; border-radius: 12px; border: 2px dashed #e2e8f0; }

.fade-enter-active, .fade-leave-active { transition: opacity 0.2s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

@media (max-width: 640px) {
  .dual-search-grid { grid-template-columns: 1fr; gap: 1rem; }
}
</style>
