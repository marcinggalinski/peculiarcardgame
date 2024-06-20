<template>
  <h1 class="deck-title">{{ deck.name }}</h1>
  <small class="deck-author">by {{ deck.author }}</small>
  <div class="deck-description">{{ deck.description }}</div>

  <CardsList :type="CardType.Black" :cards="blackCards" />
  <CardsList :type="CardType.White" :cards="whiteCards" />
</template>

<script setup lang="ts">
import { inject } from "vue";

import CardsList from "@/components/deck-management/CardsList.vue";
import { DeckManagementApiServiceKey } from "@/keys";
import { CardType } from "@/models/deck-management/api";
import DeckManagementApiService from "@/services/deck-management/apiService";

const { id } = defineProps<{
  id: number;
}>();

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
  throw new Error("DeckManagementApiService not initialized");
}

const deck = await deckManagementApiService.getDeck(id);
const cards = await deckManagementApiService.getCards(id);

const whiteCards = cards.filter(x => x.cardType === CardType.White);
const blackCards = cards.filter(x => x.cardType === CardType.Black);
</script>

<style lang="stylus">
.deck-title
  margin-bottom 0
</style>
