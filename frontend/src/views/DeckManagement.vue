<template>
  <header>
    <h1>Deck Management</h1>
  </header>
  <main>{{ decks }}</main>
</template>

<script setup lang="ts">
import { inject } from "vue";
import { DeckManagementApiServiceKey } from "@/keys";
import DeckManagementApiService from "@/services/deck-management/apiService";

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
  throw new Error("DeckManagementApiService not initialized");
}

const decks = await deckManagementApiService.getDecks();
</script>

<style scoped></style>
