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
          {{ userStore.username }}
        </div>
        <div v-else>
          {{ route.path }}
          <RouterLink :to="{ name: 'sign-in', query: { returnUrl: '/decks' } }" class="clear">Sign in</RouterLink>
        </div>
      </h1>
    </div>
  </header>
  <RouterView />
</template>

<script setup lang="ts">
import { useUserStore } from "@/stores/user";
import { RouterLink, useRoute } from "vue-router";

const route = useRoute();
const userStore = useUserStore();
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
</style>
