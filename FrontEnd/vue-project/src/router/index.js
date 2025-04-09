import { createRouter, createWebHistory } from 'vue-router';
import ConnectionView from '../components/ConnectionView.vue';
import HomeView from '../components/HomeView.vue'; // 导入新的HomeView

const routes = [
    {
        path: '/',
        name: 'home',
        component: HomeView  // 使用专门的主页组件
    },
    {
        path: '/connectionview',
        name: 'connectionview',
        component: ConnectionView
    }
];

const router = createRouter({ 
    history: createWebHistory(), 
    routes 
});

export default router;