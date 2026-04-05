<script setup>
import { ref, computed } from 'vue'
import OrderEntry from './components/OrderEntry.vue'
import SearchOrder from './components/SearchOrder.vue'
import InputCheck from './components/InputCheck.vue'
import KeyInspection from './components/KeyInspection.vue'
import OutputPacking from './components/OutputPacking.vue'

const currentTab = ref('searchOrder')

const tabs = [
  { id: 'searchOrder', name: '歷程查詢', icon: '🔎', component: SearchOrder },
  { id: 'orderEntry', name: '生產單建立', icon: '📝', component: OrderEntry }
]

const currentComponent = computed(() => {
  const tab = tabs.find(t => t.id === currentTab.value)
  return tab ? tab.component : null
})

// 增加一個隨機 Key，當點擊「生產單建立」時強制重新渲染該組件以清空欄位
const orderEntryKey = ref(0)
const handleTabClick = (tabId) => {
  if (tabId === 'orderEntry') {
    orderEntryKey.value += 1
  }
  currentTab.value = tabId
}
</script>

<template>
  <div class="app-wrapper">
    <!-- Main Header -->
    <header class="main-header no-print">
      <div class="header-content">
        <h1 class="logo-text">Stitch Flow <span>Production Management</span></h1>
      </div>
    </header>

    <!-- Content Container -->
    <div class="content-viewport">
      <transition name="fade" mode="out-in">
        <keep-alive :exclude="['OrderEntry']">
            <component 
              :is="currentComponent" 
              :key="currentTab === 'orderEntry' ? orderEntryKey : currentTab"
              @saved="handleTabClick('searchOrder')"
            />
        </keep-alive>
      </transition>
    </div>

    <!-- Bottom Nav -->
    <nav class="main-nav no-print">
      <div class="nav-container">
        <button 
          v-for="tab in tabs" 
          :key="tab.id"
          class="nav-item"
          :class="{ active: currentTab === tab.id }"
          @click="handleTabClick(tab.id)"
        >
          <span class="nav-icon">{{ tab.icon }}</span>
          <span class="nav-label">{{ tab.name }}</span>
        </button>
      </div>
    </nav>
  </div>
</template>

<style scoped>
.app-wrapper {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background-color: #f8fafc;
}

.main-header {
  background-color: #1e293b;
  color: white;
  padding: 0.85rem 1rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
}

.logo-text {
  font-size: 1.15rem;
  font-weight: 900;
  margin: 0;
  text-align: left;
  letter-spacing: 0.05rem;
}

.logo-text span {
  font-weight: 300;
  font-size: 0.8rem;
  opacity: 0.7;
  margin-left: 0.5rem;
  text-transform: uppercase;
}

.content-viewport {
  flex: 1;
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
  padding: 1rem;
  padding-bottom: 7rem;
}

.main-nav {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background-color: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(8px);
  border-top: 1px solid #e2e8f0;
  box-shadow: 0 -4px 12px rgba(0,0,0,0.05);
  z-index: 1000;
  padding: env(safe-area-inset-bottom) 0;
}

.nav-container {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: center;
  align-items: stretch;
}

.nav-item {
  background: none;
  border: none;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.2rem;
  padding: 0.65rem 0.25rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.25s;
  flex: 1;
  min-width: 0; /* 防止擠壓破版 */
}

.nav-item.active {
  color: #2563eb;
  background-color: rgba(37, 99, 235, 0.04);
}

.nav-item.active .nav-icon {
  transform: translateY(-2px);
}

.nav-icon { 
  font-size: 1.35rem; 
  transition: transform 0.2s;
}

.nav-label { 
  font-size: 0.7rem; 
  font-weight: 800;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  width: 100%;
}

@media print {
  .no-print { display: none !important; }
  .content-viewport { padding: 0 !important; margin: 0 !important; max-width: none !important; }
}

@media (min-width: 768px) {
  .nav-label { font-size: 0.8rem; }
  .nav-icon { font-size: 1.5rem; }
}
</style>
