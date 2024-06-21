import { fileURLToPath, URL } from "node:url";

import fs from "fs";
import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  process.env = { ...process.env, ...loadEnv(mode, process.cwd()) };
  return {
    plugins: [vue()],
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url)),
      },
    },
    server: {
      port: 3000,
      strictPort: true,
      watch: {
        usePolling: true,
      },
      hmr: {
        host: "localhost",
      },
      https: {
        pfx: fs.readFileSync(process.env.VITE_CERT_LOCATION!),
        passphrase: process.env.VITE_CERT_PASSPHRASE,
      },
    },
  };
});
