<template>
  <h2>
    {{ type }} cards ({{ cards.length }})
    <Button
      rounded
      text
      v-if="userStore.id == authorId"
      icon="pi pi-plus"
      class="add-card-button"
      @click="showAddCardDialog()"
    />
  </h2>
  <div v-if="cards.length" class="cards-list">
    <CardPreview
      v-for="card in cards"
      :author-id="authorId"
      :card="card"
      :deleted="deletedIds.has(card.id)"
      @update="(id, text) => updateCard(id, text)"
      @delete="id => deleteCard(id)"
      @restore="id => restoreCard(id)"
    />
  </div>
  <div v-else>No {{ type.toLocaleLowerCase() }} cards yet.</div>

  <Dialog
    modal
    id="add-card-dialog"
    v-model:visible="isAddCardDialogVisible"
    :header="`Add new ${type.toLocaleLowerCase()} card`"
    :closable="false"
  >
    <Textarea v-model="newCardText" class="edit-input" />
    <template #footer>
      <Button class="float-left" severity="secondary" label="Cancel" @click="hideAddCardDialog(false)" />
      <Button class="float-right" label="Save" @click="hideAddCardDialog(true)" />
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { ref } from "vue";

import Button from "primevue/button";
import Dialog from "primevue/dialog";
import Textarea from "primevue/textarea";

import type { CardType, GetCardResponse } from "@/models/deck-management/api";
import CardPreview from "@/components/deck-management/CardPreview.vue";
import { useUserStore } from "@/stores/user";

const { type } = defineProps<{
  authorId: number;
  cards: GetCardResponse[];
  type: CardType;
  deletedIds: Set<number>;
}>();

const emit = defineEmits<{
  (event: "add", text: string, type: CardType): void;
  (event: "update", id: number, text: string): void;
  (event: "delete", id: number): void;
  (event: "restore", id: number): void;
}>();

const userStore = useUserStore();

const isAddCardDialogVisible = ref(false);
const newCardText = ref("");

const showAddCardDialog = () => {
  newCardText.value = "";
  isAddCardDialogVisible.value = true;
};

const hideAddCardDialog = (save: boolean) => {
  if (save) {
    emit("add", newCardText.value, type);
  }
  isAddCardDialogVisible.value = false;
};

const updateCard = (id: number, text: string) => {
  emit("update", id, text);
};

const deleteCard = (id: number) => {
  emit("delete", id);
};

const restoreCard = (id: number) => {
  emit("restore", id);
};
</script>

<style lang="stylus">
.cards-list
  display flex
  flex-wrap wrap

.add-card-button
  height 29px
  width 29px

#add-card-dialog
  width 50%

  .edit-input
    width 100%
</style>
