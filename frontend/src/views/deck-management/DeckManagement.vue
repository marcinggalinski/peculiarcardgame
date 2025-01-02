<template>
  <Navbar title="Peculiar Card Game" title-href="/" subtitle="Deck Management" subtitle-href="/decks" />
  <RouterView />
</template>

<script setup lang="ts">
import Navbar from "@/components/Navbar.vue";
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

#sign-out
  cursor pointer
</style>
