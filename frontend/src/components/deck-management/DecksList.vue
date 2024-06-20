<template>
  <div id="decks-list">
    <DeckPreview v-if="decks.length" v-for="deck in decks" :deck="deck" />
    <span v-else>No decks yet!</span>
  </div>
</template>

<script setup lang="ts">
import { inject } from "vue";
import { DeckManagementApiServiceKey } from "@/keys";
import DeckManagementApiService from "@/services/deck-management/apiService";
import DeckPreview from "@/components/deck-management/DeckPreview.vue";

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
  throw new Error("DeckManagementApiService not initialized");
}

const decks = (await deckManagementApiService.getDecks()) ?? [];
</script>

<style lang="stylus">
#decks-list
  display flex
  flex-wrap wrap
</style>
