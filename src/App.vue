<script setup>
import { ref, computed, onMounted } from 'vue'
import OrderEntry from './components/OrderEntry.vue'
import SearchOrder from './components/SearchOrder.vue'
import InputCheck from './components/InputCheck.vue'
import KeyInspection from './components/KeyInspection.vue'
import OutputPacking from './components/OutputPacking.vue'

const currentTab = ref('searchOrder')

const tabs = [
  { id: 'searchOrder', name: '歷程查詢', icon: '🗂️', component: SearchOrder },
  { id: 'orderEntry', name: '生產單建立', icon: '✏️', component: OrderEntry }
]

const currentComponent = computed(() => {
  const tab = tabs.find(t => t.id === currentTab.value)
  return tab ? tab.component : null
})

const orderEntryKey = ref(0)
const handleTabClick = (tabId) => {
  if (tabId === 'orderEntry') {
    orderEntryKey.value += 1
  }
  currentTab.value = tabId
}

// 主題切換邏輯
const themes = [
  { name: '深青色', color: '#0d9488', bg: 'rgba(13, 148, 136, 0.08)', appBg: '#f0fdfa' },
  { name: '落日橘', color: '#ea580c', bg: 'rgba(234, 88, 12, 0.08)', appBg: '#fff7ed' },
  { name: '午夜紫', color: '#7c3aed', bg: 'rgba(124, 58, 237, 0.08)', appBg: '#f5f3ff' },
  { name: '翡翠綠', color: '#059669', bg: 'rgba(5, 150, 105, 0.08)', appBg: '#ecfdf5' },
  { name: '經典藍', color: '#2563eb', bg: 'rgba(37, 99, 235, 0.08)', appBg: '#eff6ff' }
]
const currentThemeIndex = ref(0)

const applyTheme = () => {
  const theme = themes[currentThemeIndex.value]
  document.documentElement.style.setProperty('--theme-color', theme.color)
  document.documentElement.style.setProperty('--theme-bg', theme.bg)
  document.documentElement.style.setProperty('--app-bg', theme.appBg)
  // 同步更新 OrderEntry 中使用的 --accent-color 以維持整體一致性
  document.documentElement.style.setProperty('--accent-color', theme.color)
}

const nextTheme = () => {
  currentThemeIndex.value = (currentThemeIndex.value + 1) % themes.length
  applyTheme()
}

onMounted(() => {
  applyTheme()
})
</script>

<template>
  <div class="app-wrapper">
    <!-- Theme Switcher (Top Right) -->
    <button class="theme-switcher no-print" @click="nextTheme" :title="'切換主題 (' + themes[currentThemeIndex].name + ')'">
      🎨
    </button>
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
  background-color: var(--app-bg, #f8fafc);
  transition: background-color 0.4s ease;
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
  color: var(--theme-color, #0d9488);
  background-color: var(--theme-bg, rgba(13, 148, 136, 0.08));
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

.theme-switcher {
  position: fixed;
  top: 1rem;
  right: 1rem;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 50%;
  width: 44px;
  height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.3rem;
  cursor: pointer;
  box-shadow: 0 4px 6px rgba(0,0,0,0.05);
  z-index: 1000;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.theme-switcher:hover {
  transform: scale(1.1) rotate(15deg);
  box-shadow: 0 6px 12px rgba(0,0,0,0.1);
}

@media (min-width: 768px) {
  .nav-label { font-size: 0.8rem; }
  .nav-icon { font-size: 1.5rem; }
}
</style>
