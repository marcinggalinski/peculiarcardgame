<template>
  <h2>
    {{ type }} cards ({{ cards.length }})
    <Button v-if="userStore.id == authorId" icon="pi pi-plus" rounded text @click="addCard()" />
  </h2>
  <div v-if="cards.length" class="cards-list">
    <CardPreview
      v-for="card in cards"
      :author-id="authorId"
      :card="card"
      @update="(id, text) => updateCard(id, text)"
    />
  </div>
  <div v-else>No {{ type.toLocaleLowerCase() }} cards yet.</div>
</template>

<script setup lang="ts">
import Button from "primevue/button";

import type { CardType, GetCardResponse } from "@/models/deck-management/api";
import CardPreview from "@/components/deck-management/CardPreview.vue";
import { useUserStore } from "@/stores/user";

defineProps<{
  authorId: number;
  cards: GetCardResponse[];
  type: CardType;
}>();

const emit = defineEmits<{
  (event: "update", id: number, text: string): void;
}>();

const userStore = useUserStore();

const addCard = () => {
  alert("adding card");
};

const updateCard = (id: number, text: string) => {
  emit("update", id, text);
};
</script>

<style lang="stylus">
.cards-list
  display flex
  flex-wrap wrap
</style>
