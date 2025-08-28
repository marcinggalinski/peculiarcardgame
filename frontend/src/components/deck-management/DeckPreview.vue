<template>
  <div v-if="deck" class="deck-preview">
    <RouterLink :to="{ name: 'deck-editor', params: { id: deck.id } }" class="clear">
      <div class="deck-preview-header">
        <span class="deck-preview-name">{{ deck.name }}</span>
        <small class="deck-preview-author">by {{ deck.author }}</small>
      </div>
      <div class="deck-preview-content">
        <div class="deck-preview-description">{{ deck.description || "No description" }}</div>
      </div>
      <div class="deck-preview-footer">
        <div>
          {{ deck.blackCardCount + deck.whiteCardCount }} cards ({{ deck.blackCardCount }} black,
          {{ deck.whiteCardCount }} white)
        </div>
      </div>
    </RouterLink>
    <Button
      v-if="userStore.id == deck.authorId"
      rounded
      text
      icon="pi pi-trash"
      class="deck-preview-delete-button"
      @click="deleteDeck()"
    />
  </div>
</template>

<script setup lang="ts">
import { inject } from "vue";

import Button from "primevue/button";
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";

import { DeckManagementApiServiceKey } from "@/keys";
import type { GetDeckResponse } from "@/models/deck-management/api";
import type DeckManagementApiService from "@/services/deck-management/apiService";
import { useUserStore } from "@/stores/user";

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
  throw new Error("DeckManagementApiService not initialized");
}

const { deck } = defineProps<{
  deck: GetDeckResponse;
}>();

const emit = defineEmits<{
  (event: "delete", id: number): void;
}>();

const confirm = useConfirm();
const toast = useToast();
const userStore = useUserStore();

const deleteDeck = () => {
  confirm.require({
    header: `Deleting deck ${deck.name}`,
    message: "Are you sure you want to delete this deck?",
    acceptIcon: "pi pi-trash",
    acceptClass: "float-right accept-button",
    accept: async () => {
      try {
        await deckManagementApiService.deleteDeck(deck.id);
        emit("delete", deck.id);
        toast.add({
          summary: "Success",
          detail: "Deck deleted",
          severity: "success",
          life: 3000,
        });
      } catch (error) {
        console.error("Error when deleting deck:");
        console.error(error);
        toast.add({
          summary: "An error has occurred",
          detail: error?.toString(),
          severity: "error",
          life: 3000,
        });
      }
    },

    rejectIcon: "pi pi-times",
    rejectClass: "float-left reject-button",
  });
};
</script>

<style scoped lang="stylus">
.deck-preview
  max-width 250px
  max-height 350px
  width 250px
  height 350px

  display flex
  flex-direction column

  border 1px black solid
  border-radius 15px

  margin 5px
  position relative

  a
    height 100%

  .deck-preview-header
    border-radius 15px 15px 0 0

    margin -1px
    padding 15px

    background black
    color white

    .deck-preview-name
      font-weight bold
      display block

      overflow hidden
      text-overflow ellipsis
      white-space nowrap

    .deck-preview-author
      display block

  .deck-preview-content
    height 100%
    max-height 237px

    margin -1px
    padding 15px

    .deck-preview-description
      display -webkit-box
      -webkit-box-orient vertical
      -webkit-line-clamp 10

      white-space pre-wrap
      overflow hidden
      text-overflow ellipsis

      max-height 100%
      font-style italic

  .deck-preview-footer
    border-radius 0 0 15px 15px

    margin auto -1px -1px -1px
    padding 15px

    background black
    color white

  .deck-preview-delete-button
    position absolute
    top 0
    right 0

    color white
    background none
    transition 0.3s ease
    opacity 0

.deck-preview:hover
  .deck-preview-delete-button
    transition 0.3s ease
    opacity 1
</style>
<style lang="stylus">

.accept-button
  background red
  border-color red

.reject-button
  color #020617
  background none
</style>
