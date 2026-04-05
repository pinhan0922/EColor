<script setup>
import { ref, computed } from 'vue'

const photoPreview = ref(null)
const chestMeasurement = ref(0)
const lengthMeasurement = ref(0)
const stdChest = 100 
const stdLength = 65 

const chestError = computed(() => chestMeasurement.value - stdChest)
const lengthError = computed(() => lengthMeasurement.value - stdLength)

const getStatusColor = (error) => {
  if (Math.abs(error) <= 1) return 'text-success'
  if (Math.abs(error) <= 3) return 'text-warning'
  return 'text-danger'
}

const handleFileUpload = (event) => {
  const file = event.target.files[0]
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => { photoPreview.value = e.target.result }
    reader.readAsDataURL(file)
  }
}

const saveInspection = () => {
  alert('關鍵檢驗資料已儲存！')
}
</script>

<template>
  <div class="document-section fade-in">
    <h2 class="section-title-doc">生產關鍵檢驗 (首件)</h2>

    <div class="main-doc-content">
      <div class="photo-doc-side">
        <h3 class="card-label">首件樣衣照片</h3>
        <div class="doc-photo-box" :class="{ 'has-image': photoPreview }">
          <input type="file" @change="handleFileUpload" class="hidden-file" id="insp-upload" />
          <label for="insp-upload" v-if="!photoPreview" class="photo-placeholder">
            <span>📷</span>
            <p>拍攝首件</p>
          </label>
          <img v-else :src="photoPreview" class="doc-img" />
        </div>
      </div>

      <div class="measurement-doc-side">
        <h3 class="card-label">尺寸測量紀錄</h3>
        <div class="doc-input-table">
          <div class="doc-row">
            <div class="doc-col-label">
              <span class="m-label-doc">胸圍</span>
              <span class="std-label-doc">標準: {{ stdChest }}cm</span>
            </div>
            <div class="doc-col-input">
              <input type="number" step="0.5" v-model="chestMeasurement" class="form-input-doc text-center" />
            </div>
            <div class="doc-col-status">
              <span class="error-badge-doc" :class="getStatusColor(chestError)" v-if="chestMeasurement > 0">
                {{ chestError > 0 ? '+' : '' }}{{ chestError }} cm
              </span>
            </div>
          </div>

          <div class="doc-row mt-3">
            <div class="doc-col-label">
              <span class="m-label-doc">衣長</span>
              <span class="std-label-doc">標準: {{ stdLength }}cm</span>
            </div>
            <div class="doc-col-input">
              <input type="number" step="0.5" v-model="lengthMeasurement" class="form-input-doc text-center" />
            </div>
            <div class="doc-col-status">
              <span class="error-badge-doc" :class="getStatusColor(lengthError)" v-if="lengthMeasurement > 0">
                {{ lengthError > 0 ? '+' : '' }}{{ lengthError }} cm
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="mt-4">
      <button class="btn btn-primary w-100" @click="saveInspection">確認儲存檢驗數據</button>
    </div>
  </div>
</template>

<style scoped>
.document-section {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  text-align: left;
}

.section-title-doc {
  text-align: left;
  font-size: 1.5rem;
  border-bottom: 2px solid #000;
  padding-bottom: 0.5rem;
  margin-bottom: 1.5rem;
}

.main-doc-content {
  display: flex;
  gap: 2rem;
}

.photo-doc-side { width: 220px; }

.measurement-doc-side { flex: 1; }

.doc-photo-box {
  border: 2px dashed #cbd5e1;
  height: 250px;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
  border-radius: 4px;
}

.hidden-file { display: none; }

.photo-placeholder {
  cursor: pointer;
  text-align: center;
}

.photo-placeholder span { font-size: 2rem; display: block; }

.doc-img { width: 100%; height: 100%; object-fit: contain; }

.card-label {
  font-size: 1.1rem;
  margin-bottom: 1rem;
  font-weight: 700;
}

.doc-input-table {
  border: 1px solid #e2e8f0;
  padding: 1.5rem;
  border-radius: 6px;
}

.doc-row {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.doc-col-label { width: 100px; display: flex; flex-direction: column; }
.m-label-doc { font-weight: 700; font-size: 1.1rem; }
.std-label-doc { font-size: 0.75rem; color: #64748b; }

.doc-col-input { flex: 1; }
.form-input-doc {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 4px;
  font-size: 1.25rem;
  font-weight: bold;
}
.text-center { text-align: center; }

.doc-col-status { width: 80px; text-align: right; }
.error-badge-doc {
  font-weight: 800;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.9rem;
}

.text-success { color: #16a34a; background: #f0fdf4; border: 1px solid #16a34a; }
.text-warning { color: #ca8a04; background: #fefce8; border: 1px solid #ca8a04; }
.text-danger { color: #dc2626; background: #fef2f2; border: 1px solid #dc2626; }

.w-100 { width: 100%; }
.mt-3 { margin-top: 1rem; }
.mt-4 { margin-top: 2rem; }

@media (max-width: 600px) {
  .main-doc-content { flex-direction: column; }
  .photo-doc-side { width: 100%; }
}
</style>
