<script setup>
import { ref } from 'vue'

const checklist = ref([
  { id: 1, label: '無明顯污漬、色斑', checked: false },
  { id: 2, label: '無斷針、車縫完整', checked: false },
  { id: 3, label: '無線頭殘留', checked: false },
  { id: 4, label: '熨燙平整', checked: false },
])

const qualifiedCount = ref(0)
const defectiveCount = ref(0)

const boxPhoto = ref(null)
const billPhoto = ref(null)

const handleUpload = (type, e) => {
  const file = e.target.files[0]
  if (file) {
    const reader = new FileReader(); reader.onload = (ev) => {
      if (type === 'box') boxPhoto.value = ev.target.result;
      else billPhoto.value = ev.target.result;
    }; reader.readAsDataURL(file);
  }
}

const confirmPacking = () => {
  alert('裝箱資料已確認並上傳！')
}
</script>

<template>
  <div class="document-section fade-in">
    <h2 class="section-title-doc">出貨檢驗與入箱</h2>

    <div class="doc-card mb-4">
      <h3 class="card-label">最終規格檢核表</h3>
      <div class="doc-checklist">
        <label v-for="item in checklist" :key="item.id" class="doc-check-item">
          <input type="checkbox" v-model="item.checked" />
          <span>{{ item.label }}</span>
        </label>
      </div>
    </div>

    <div class="doc-card mb-4">
      <h3 class="card-label">件數總計</h3>
      <div class="doc-stats-grid">
        <div class="stat-doc-item doc-success">
          <label>合格品數</label>
          <div class="stat-count-control">
            <button @click="qualifiedCount = Math.max(0, qualifiedCount-1)">-</button>
            <input type="number" v-model="qualifiedCount" />
            <button @click="qualifiedCount++">+</button>
          </div>
        </div>
        <div class="stat-doc-item doc-danger">
          <label>次品/報廢</label>
          <div class="stat-count-control">
            <button @click="defectiveCount = Math.max(0, defectiveCount-1)">-</button>
            <input type="number" v-model="defectiveCount" />
            <button @click="defectiveCount++">+</button>
          </div>
        </div>
      </div>
    </div>

    <div class="doc-card mb-4">
      <h3 class="card-label">出貨影像紀錄</h3>
      <div class="doc-photos-grid">
        <div class="doc-photo-unit">
          <label>箱內照紀錄</label>
          <div class="doc-photo-box mini" :class="{ 'has-image': boxPhoto }">
            <input type="file" @change="e => handleUpload('box', e)" class="hidden-file" id="box-upload" />
            <label for="box-upload" v-if="!boxPhoto" class="photo-placeholder-mini">點擊拍攝</label>
            <img v-else :src="boxPhoto" class="doc-img" />
          </div>
        </div>
        <div class="doc-photo-unit">
          <label>託運單紀錄</label>
          <div class="doc-photo-box mini" :class="{ 'has-image': billPhoto }">
            <input type="file" @change="e => handleUpload('bill', e)" class="hidden-file" id="bill-upload" />
            <label for="bill-upload" v-if="!billPhoto" class="photo-placeholder-mini">點擊拍攝</label>
            <img v-else :src="billPhoto" class="doc-img" />
          </div>
        </div>
      </div>
    </div>

    <div class="mt-4">
      <button class="btn btn-primary w-100" @click="confirmPacking">完成封箱與出貨上傳</button>
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

.doc-card {
  padding: 1rem;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
}

.card-label {
  font-size: 1rem;
  margin-bottom: 0.75rem;
  color: #64748b;
  font-weight: 700;
}

.doc-checklist {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

.doc-check-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 4px;
}

.doc-stats-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.stat-doc-item {
  padding: 1rem;
  border-radius: 6px;
  text-align: center;
}

.doc-success { background: #f0fdf4; border: 1px solid #16a34a; }
.doc-danger { background: #fef2f2; border: 1px solid #dc2626; }

.stat-count-control {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.stat-count-control button {
  width: 30px;
  height: 30px;
  background: white;
  border: 1px solid #ccc;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.stat-count-control input {
  width: 60px;
  text-align: center;
  font-size: 1.25rem;
  font-weight: bold;
  border: none;
  background: transparent;
}

.doc-photos-grid {
  display: flex;
  gap: 1rem;
}

.doc-photo-unit { flex: 1; text-align: center; }
.doc-photo-unit label { font-size: 0.8rem; color: #64748b; margin-bottom: 0.25rem; display: block; }

.doc-photo-box.mini {
  height: 120px;
  border: 1px dashed #cbd5e1;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
}

.hidden-file { display: none; }
.photo-placeholder-mini { cursor: pointer; color: #2563eb; text-decoration: underline; font-size: 0.9rem; }
.doc-img { width: 100%; height: 100%; object-fit: cover; }

.w-100 { width: 100%; }
.mt-4 { margin-top: 2rem; }

@media (max-width: 500px) {
  .doc-checklist { grid-template-columns: 1fr; }
  .doc-stats-grid { grid-template-columns: 1fr; }
}
</style>
