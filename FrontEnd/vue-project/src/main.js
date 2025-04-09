import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'

// Vue 3 的正确初始化方式
createApp(App)
  .use(router)  // 使用路由
  .mount('#app')