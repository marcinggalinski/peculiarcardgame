<template>
  <header>
    <h2>Welcome to</h2>
    <h1>Peculiar Card Game</h1>
    <h3>A <a href="https://www.cardsagainsthumanity.com">Cards Against Humanity</a> clone</h3>
  </header>
  <main>
    <RouterLink :to="{ name: 'decks-list' }">Deck Management</RouterLink>
  </main>
</template>

<script setup lang="ts"></script>

<style scoped lang="stylus">
header
  background-color black
  color white
  text-align center
  padding 10px

  h2
    margin 0

  h1
    margin 0
</style>
