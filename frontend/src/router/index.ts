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
      path: "/sign-in",
      name: "sign-in",
      component: () => import("../views/SignIn.vue"),
      props: route => ({ returnUrl: route.query.returnUrl }),
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
          component: () => import("../views/deck-management/DeckEditor.vue"),
          props: route => ({ id: Number(route.params.id) }),
        },
      ],
    },
  ],
});

export default router;
