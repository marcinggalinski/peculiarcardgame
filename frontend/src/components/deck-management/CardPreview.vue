<template>
  <div class="card-preview" :class="colorClass">
    <div class="card-preview-content" :class="deletedClass">
      <div class="card-preview-text">{{ card.text.replaceAll("_", "_____") }}</div>
      <div v-if="card.cardType === CardType.Black" class="card-preview-picks">Pick {{ picks }}</div>
    </div>
    <Button
      v-if="userStore.id == authorId && !deleted"
      rounded
      text
      icon="pi pi-trash"
      class="card-preview-delete-button"
      :class="colorClass"
      @click="deleteCard()"
    />
    <Button
      v-if="userStore.id == authorId && !deleted"
      rounded
      text
      icon="pi pi-pencil"
      class="card-preview-edit-button"
      :class="colorClass"
      @click="showEditTextDialog()"
    />
    <Button
      v-if="userStore.id == authorId && deleted"
      rounded
      text
      icon="pi pi-refresh"
      class="card-preview-restore-button"
      :class="colorClass"
      @click="restoreCard()"
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
import { computed, ref, toRefs } from "vue";

import Button from "primevue/button";
import Dialog from "primevue/dialog";
import Textarea from "primevue/textarea";

import type { GetCardResponse } from "@/models/deck-management/api";
import { CardType } from "@/models/deck-management/api";
import { useUserStore } from "@/stores/user";

const props = defineProps<{
  authorId: number;
  card: GetCardResponse;
  deleted: boolean;
}>();

const { card, deleted } = toRefs(props);

const emit = defineEmits<{
  (event: "update", id: number, text: string): void;
  (event: "delete", id: number): void;
  (event: "restore", id: number): void;
}>();

const userStore = useUserStore();

const colorClass = card.value.cardType.toLocaleLowerCase();
const deletedClass = computed(() => (deleted.value ? "blurred" : ""));
const picks = card.value.cardType === CardType.Black ? card.value.text.split("_").length - 1 || 1 : 0;

const isEditCardDialogVisible = ref(false);
const tempText = ref(card.value.text);

const deleteCard = () => {
  emit("delete", card.value.id);
};

const restoreCard = () => {
  emit("restore", card.value.id);
};

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
  flex 1 0 20%
  max-width 150px
  max-height 210px
  aspect-ratio 5 / 7

  border 1px black solid
  border-radius 15px

  margin 5px
  padding 15px
  position relative

  .card-preview-content
    width 100%
    height 100%
    display flex
    flex-direction column

    white-space: pre-wrap

    transition 0.3s ease

    .card-preview-content
      height 100%

    .card-preview-picks
      margin-top auto
      text-align right

  .card-preview-delete-button
    position absolute
    top 0
    right 0

    background none
    transition 0.3s ease
    opacity 0

  .card-preview-edit-button
    position absolute
    top 30px
    right 0

    background none
    transition 0.3s ease
    opacity 0

  .card-preview-restore-button
    position absolute
    top 0
    right 0

    background none
    transition 0.3s ease
    opacity 0

.card-preview:hover
  .card-preview-delete-button
    transition 0.3s ease
    opacity 1

  .card-preview-edit-button
    transition 0.3s ease
    opacity 1

  .card-preview-restore-button
    transition 0.3s ease
    opacity 1

.black
  background black
  color white

.white
  background white
  color black

.blurred
  transition 0.3s ease
  filter blur(3px)

#edit-card-dialog
  width 50%

  .edit-input
    width 100%
</style>
