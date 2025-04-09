import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  define: {
    'process.env': process.env
  },
  server: {
    proxy: {
      '/guacamole': {
        target: 'http://192.168.0.209:8080/guacamole',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/guacamole/, '')
      },
      '/wsguacamole': {
        target: 'ws://192.168.0.209:8080/guacamole',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/wsguacamole/, '')
      }
    }
  },
})
