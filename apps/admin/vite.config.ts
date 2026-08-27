import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import ui from '@tabtab/ui/vite'
import path from 'node:path'

export default defineConfig({
  plugins: [
    vue(),
    ui(),
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 3001,
    host: true,
    proxy: { '/api': { target: 'http://localhost:5080', changeOrigin: true } },
  },
})
