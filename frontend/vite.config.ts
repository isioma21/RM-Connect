import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// Dev: forward /api to the .NET API (same origin, no CORS)
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': process.env.API_URL ?? 'http://localhost:5126',
    },
  },
})
