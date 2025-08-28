import "primevue/resources/themes/aura-light-noir/theme.css";
import "primeicons/primeicons.css";
import "@/assets/main.stylus";

import { createPinia } from "pinia";
import { createApp } from "vue";

import PrimeVue from "primevue/config";
import ToastService from "primevue/toastservice";

import App from "@/App.vue";
import router from "@/router";

const app = createApp(App);
const pinia = createPinia();

app.use(router);
app.use(pinia);

app.use(PrimeVue);
app.use(ToastService);

app.mount("#app");
