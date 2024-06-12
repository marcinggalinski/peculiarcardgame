import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: () => import("../views/LandingPage.vue"),
    },
    {
      path: "/decks",
      name: "deck-management",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("../views/deck-management/DeckManagement.vue"),
      children: [
        {
          path: "",
          name: "decks-list",
          component: () => import("../components/deck-management/DecksList.vue"),
        },
        {
          path: ":id",
          name: "deck-editor",
          props: route => ({ id: Number(route.params.id) }),
          component: () => import("../views/deck-management/DeckEditor.vue"),
        },
      ],
    },
  ],
});

export default router;
