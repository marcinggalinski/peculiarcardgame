import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/LandingPage.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: HomeView,
    },
    {
      path: "/deck-management",
      name: "deck-management",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("../views/deck-management/DeckManagement.vue"),
    },
    {
      path: "/deck-management/:id",
      name: "deck-editor",
      props: route => ({ id: Number(route.params.id) }),
      component: () => import("../views/deck-management/DeckEditor.vue")
    }
  ],
});

export default router;
