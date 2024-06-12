<template>
  <header>
    <RouterLink :to="{ name: 'decks-list' }" class="clear topbar-item">
      <h1 id="deck-management-logo">Deck Management test</h1>
    </RouterLink>
    <RouterLink :to="{ name: 'home' }" class="clear topbar-item">
      <h1 id="deck-management-pcg-logo">Peculiar Card Game</h1>
    </RouterLink>
    <h1 id="deck-management-filler" />
  </header>
  <RouterView />
</template>

<script setup lang="ts"></script>

<style scoped lang="stylus">
header
  display flex
  justify-content center
  background-color black
  color white
  padding 10px

  .topbar-item, h1
    flex 1
    margin 0

  #deck-management-pcg-logo
    text-align center

#decks-list
  display flex
  flex-wrap wrap
</style>
