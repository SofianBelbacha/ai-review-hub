import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    lib: {
      entry:    'src/widget.ts',
      name:     'AiReviewHub',
      fileName: 'widget',
      formats:  ['iife'], // script auto-exécutable
    },
    outDir:   'dist',
    rollupOptions: {
      output: {
        inlineDynamicImports: true,
      }
    },
    minify:     'terser',
    sourcemap:  false,
  }
});