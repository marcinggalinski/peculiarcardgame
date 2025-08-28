<template>
  <div v-if="userStore.isSignedIn" class="padding">
    <h1>
      Your decks ({{ ownDecks.length }})
      <Button rounded text icon="pi pi-plus" id="add-deck-button" @click="showAddDeckDialog()" />
    </h1>
    <div class="decks-list">
      <DeckPreview v-if="ownDecks.length" v-for="deck in ownDecks" :deck="deck" @delete="refreshDecks()" />
      <span v-else>You haven't created any deck yet!</span>
    </div>
    <h1>Others decks</h1>
    <div class="decks-list">
      <DeckPreview v-if="othersDecks.length" v-for="deck in othersDecks" :deck="deck" />
      <span v-else>No decks yet!</span>
    </div>
  </div>
  <div v-else class="padding">
    <h1>Decks</h1>
    <div class="decks-list">
      <DeckPreview v-if="othersDecks.length" v-for="deck in othersDecks" :deck="deck" />
      <span v-else>No decks yet!</span>
    </div>
  </div>

  <Dialog modal id="add-deck-dialog" v-model:visible="isAddDeckDialogVisible" header="Add new deck" :closable="false">
    <div class="field">
      <div>Name <small class="red">*</small></div>
      <InputText v-model="newDeckName" :disabled="isLoading" class="edit-input" />
    </div>
    <div class="field">
      <div class="margin-top">Description</div>
      <Textarea v-model="newDeckDescription" :disabled="isLoading" class="edit-input" />
    </div>

    <small class="red">* Field is required</small>

    <template #footer>
      <Button
        class="float-left"
        severity="secondary"
        label="Cancel"
        icon="pi pi-times"
        :disabled="isLoading"
        @click="hideAddDeckDialog(false)"
      />
      <Button
        class="float-right"
        label="Save"
        icon="pi pi-check"
        :loading="isLoading"
        :disabled="isLoading || !newDeckName"
        @click="hideAddDeckDialog(true)"
      />
    </template>
  </Dialog>

  <ConfirmDialog :draggable="false" />
</template>

<script setup lang="ts">
import { computed, inject, ref } from "vue";
import { useRouter } from "vue-router";

import Button from "primevue/button";
import ConfirmDialog from "primevue/confirmdialog";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";
import { useToast } from "primevue/usetoast";

import { DeckManagementApiServiceKey } from "@/keys";
import DeckPreview from "@/components/deck-management/DeckPreview.vue";
import type { GetDeckResponse } from "@/models/deck-management/api";
import type DeckManagementApiService from "@/services/deck-management/apiService";
import { useUserStore } from "@/stores/user";

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
  throw new Error("DeckManagementApiService not initialized");
}

const router = useRouter();
const toast = useToast();
const userStore = useUserStore();

const isAddDeckDialogVisible = ref(false);
const newDeckName = ref("");
const newDeckDescription = ref("");
const isLoading = ref(false);
const decks = ref<GetDeckResponse[]>([]);

const ownDecks = computed(() => decks.value.filter(x => x.authorId === userStore.id));
const othersDecks = computed(() => decks.value.filter(x => x.authorId !== userStore.id));

const refreshDecks = async () => {
  decks.value = (await deckManagementApiService.getDecks()) ?? [];
};

const showAddDeckDialog = () => {
  newDeckName.value = "";
  newDeckDescription.value = "";
  isAddDeckDialogVisible.value = true;
};

const hideAddDeckDialog = async (save: boolean) => {
  if (save) {
    isLoading.value = true;

    try {
      const deck = await deckManagementApiService.addDeck(newDeckName.value, newDeckDescription.value);
      toast.add({
        summary: "Success",
        detail: "Deck created",
        severity: "success",
        life: 3000,
      });
      router.push({
        name: "deck-editor",
        params: { id: deck.id },
      });
    } catch (error) {
      console.error("Error when creating deck:");
      console.error(error);

      toast.add({
        summary: "An error has occurred",
        detail: error?.toString(),
        severity: "error",
        life: 3000,
      });
    }

    isLoading.value = false;
  }
  isAddDeckDialogVisible.value = false;
};

await refreshDecks();
</script>

<style scoped lang="stylus">
.padding
  padding 10px

.decks-list
  display flex
  flex-wrap wrap
  align-items flex-start

#add-deck-dialog
  .edit-input
    width 100%

  .field
    margin-bottom 15px

  .field:last-of-type
    margin-bottom unset

.accept-button
  background red
</style>
<style lang="stylus">
#add-deck-dialog
  width 50%
</style>
