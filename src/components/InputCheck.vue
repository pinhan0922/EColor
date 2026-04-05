<script setup>
import { ref } from 'vue'

const isMaterialReady = ref(true)
const cutPiecesCount = ref(0)
const anomalyNotes = ref('')

const saveCheck = () => {
  alert('清點紀錄已儲存！')
}
</script>

<template>
  <div class="document-section fade-in">
    <h2 class="section-title-doc">進料與裁片清點</h2>
    
    <div class="doc-card mb-4">
      <h3 class="card-label">材料狀態</h3>
      <div class="status-selector">
        <button 
          @click="isMaterialReady = true" 
          :class="{ active: isMaterialReady, 'btn-success': isMaterialReady }"
          class="status-btn"
        >材料已到齊</button>
        <button 
          @click="isMaterialReady = false" 
          :class="{ active: !isMaterialReady, 'btn-warning': !isMaterialReady }"
          class="status-btn"
        >缺料中</button>
      </div>
    </div>

    <div class="doc-card mb-4">
      <h3 class="card-label">裁片數量清點</h3>
      <div class="doc-counter">
        <button @click="cutPiecesCount = Math.max(0, cutPiecesCount - 10)">-10</button>
        <input type="number" v-model="cutPiecesCount" class="counter-input-doc" />
        <button @click="cutPiecesCount += 10">+10</button>
      </div>
    </div>

    <div class="doc-card mb-4">
      <h3 class="card-label">異狀備註</h3>
      <textarea 
        class="form-input-doc" 
        v-model="anomalyNotes" 
        placeholder="請輸入色差或其他異常說明..."
        rows="4"
      ></textarea>
    </div>

    <div class="mt-4">
      <button class="btn btn-primary w-100" @click="saveCheck">提交清點紀錄</button>
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
  font-weight: 600;
}

.status-selector {
  display: flex;
  gap: 1rem;
}

.status-btn {
  flex: 1;
  padding: 0.75rem;
  border: 1px solid #e2e8f0;
  background: #f8fafc;
  cursor: pointer;
  border-radius: 4px;
}

.btn-success.active { background: #16a34a; color: white; border-color: #16a34a; }
.btn-warning.active { background: #ca8a04; color: white; border-color: #ca8a04; }

.doc-counter {
  display: flex;
  align-items: center;
  gap: 1rem;
  justify-content: center;
}

.doc-counter button {
  width: 50px;
  height: 50px;
  font-size: 1.25rem;
  background: #f1f5f9;
  border: 1px solid #cbd5e1;
  border-radius: 50%;
  cursor: pointer;
}

.counter-input-doc {
  width: 120px;
  text-align: center;
  font-size: 1.75rem;
  font-weight: 800;
  border: none;
  border-bottom: 2px solid #2563eb;
}

.form-input-doc {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 4px;
  font-size: 1rem;
}

.w-100 { width: 100%; }
.mt-4 { margin-top: 1rem; }
</style>
