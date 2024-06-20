<template>
  <h2>{{ type }} cards</h2>
  <div v-if="cards.length" class="cards-list">
    <CardPreview v-for="card in cards" :card="card" />
  </div>
  <div v-else>No {{ type.toLocaleLowerCase() }} cards yet.</div>
</template>

<script setup lang="ts">
import type { CardType, GetCardResponse } from "@/models/deck-management/api";
import CardPreview from "@/components/deck-management/CardPreview.vue";

defineProps<{
  cards: GetCardResponse[];
  type: CardType;
}>();
</script>

<style lang="stylus">
.cards-list
  display flex
  flex-wrap wrap
</style>
