<template>
  <div id="app">
    <!-- 全局导航/页头等可以放在这里 -->
    <router-view></router-view> <!-- 这里会显示当前路由的组件 -->
  </div>
</template>

<style>
/* 全局样式可以放在这里 */
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  margin: 0;
  padding: 0;
}
</style>