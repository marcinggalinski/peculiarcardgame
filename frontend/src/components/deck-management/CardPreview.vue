<template>
  <div class="card-preview" :class="className">
    <div class="card-preview-text">{{ card.text.replaceAll("_", "_____") }}</div>
    <div v-if="card.cardType === CardType.Black" class="card-preview-picks">Pick {{ picks }}</div>
    <Button
      v-if="userStore.id == authorId"
      rounded
      text
      icon="pi pi-pencil"
      class="card-preview-edit-button"
      :class="className"
      @click="showEditTextDialog()"
    />

    <Dialog
      modal
      id="edit-card-dialog"
      v-model:visible="isEditCardDialogVisible"
      :closable="false"
      header="Edit card text"
    >
      <Textarea v-model="tempText" class="edit-input" />
      <template #footer>
        <Button class="float-left" severity="secondary" label="Cancel" @click="hideEditTextDialog(false)" />
        <Button class="float-right" label="Save" @click="hideEditTextDialog(true)" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, toRefs } from "vue";

import Button from "primevue/button";
import Dialog from "primevue/dialog";
import Textarea from "primevue/textarea";

import type { GetCardResponse } from "@/models/deck-management/api";
import { CardType } from "@/models/deck-management/api";
import { useUserStore } from "@/stores/user";

const props = defineProps<{
  authorId: number;
  card: GetCardResponse;
}>();

const { card } = toRefs(props);

const emit = defineEmits<{
  (event: "update", id: number, text: string): void;
}>();

const userStore = useUserStore();

const className = card.value.cardType === CardType.Black ? "black" : "white";
const picks = card.value.cardType === CardType.Black ? card.value.text.split("_").length - 1 || 1 : 0;

const isEditCardDialogVisible = ref(false);
const tempText = ref(card.value.text);

const showEditTextDialog = () => {
  tempText.value = card.value.text;
  isEditCardDialogVisible.value = true;
};

const hideEditTextDialog = (save: boolean) => {
  if (save) {
    emit("update", card.value.id, tempText.value.trim());
  }
  isEditCardDialogVisible.value = false;
};
</script>

<style lang="stylus">
.card-preview
  display flex
  flex-direction column
  position relative

  flex 1 0 20%
  max-width 150px
  max-height 210px
  aspect-ratio 5 / 7

  border 1px black solid
  border-radius 15px

  margin 5px
  padding 15px

  white-space: pre-wrap

  .card-preview-picks
    margin-top auto
    text-align right

  .card-preview-edit-button
    position absolute
    top 0
    right 0

    background none
    visibility hidden
    transition 0.3s ease
    opacity 0

.card-preview:hover
  .card-preview-edit-button
    visibility visible
    transition 0.3s ease
    opacity 1

.black
  background black
  color white

.white
  background white
  color black

#edit-card-dialog
  width 50%

  .edit-input
    width 100%
</style>
