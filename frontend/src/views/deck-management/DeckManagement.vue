<template>
  <header>
    <div id="deck-management-logo" class="topbar-item">
      <RouterLink :to="{ name: 'decks-list' }" class="clear">
        <h1>Deck Management</h1>
      </RouterLink>
    </div>

    <div id="deck-management-pcg-logo" class="topbar-item">
      <RouterLink :to="{ name: 'decks-list' }" class="clear">
        <h1>Peculiar Card Game</h1>
      </RouterLink>
    </div>

    <div id="deck-management-user" class="topbar-item">
      <h1>
        <div v-if="userStore.isSignedIn">
          Signed in as {{ userStore.username }} | <a @click="signOut()" id="sign-out">Sign out</a>
        </div>
        <div v-else>
          <RouterLink :to="{ name: 'sign-in', query: { returnUrl: route.path } }" class="clear">Sign in</RouterLink>
        </div>
      </h1>
    </div>
  </header>
  <RouterView />
</template>

<script setup lang="ts">
import { RouterLink, useRoute } from "vue-router";

import { useToast } from "primevue/usetoast";

import { useUserStore } from "@/stores/user";

const route = useRoute();
const toast = useToast();
const userStore = useUserStore();

const signOut = () => {
  userStore.signOut();
  toast.add({
    summary: "Success",
    detail: "You are now signed out.",
    severity: "success",
    life: 3000,
  });
};
</script>

<style scoped lang="stylus">
header
  display flex
  justify-content center
  background-color black
  color white
  padding 10px

  .topbar-item
    flex 1
    margin 0

  a, h1
    margin 0
    display inline-block

  #deck-management-pcg-logo
    text-align center

  #deck-management-user
    text-align right

#decks-list
  display flex
  flex-wrap wrap

#sign-out
  cursor pointer
</style>
