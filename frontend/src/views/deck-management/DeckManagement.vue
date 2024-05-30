<template>
  <header>
    <h1>Peculiar Card Game</h1>
    <h1 class="move-right">Deck Management</h1>
  </header>
  <div id="decks-list">
    <DeckPreview v-for="deck in decks" :deck="deck" />
  </div>
</template>

<script setup lang="ts">
import { inject } from "vue";
import { DeckManagementApiServiceKey } from "@/keys";
import DeckManagementApiService from "@/services/deck-management/apiService";
import DeckPreview from "@/components/deck-management/DeckPreview.vue"

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
  throw new Error("DeckManagementApiService not initialized");
}

const decks = await deckManagementApiService.getDecks();
</script>

<style scoped lang="stylus">
header
  display flex
  background-color black
  color white
  text-align center
  padding 10px

  h1
    margin 0

  .move-right
    margin-left auto

#decks-list
  display flex
  flex-wrap wrap
</style>
