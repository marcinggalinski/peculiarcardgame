<template>
  <h1 class="deck-name">
    {{ deck.name }}
    <Button v-if="userStore.id == deck.authorId" icon="pi pi-pencil" rounded text @click="showEditNameDialog()" />
  </h1>
  <small class="deck-author">
    by {{ deck.author }}
    <span v-if="userStore.id == deck.authorId">(you)</span>
  </small>
  <div class="deck-description">
    {{ deck.description }}
    <Button
      v-if="userStore.id == deck.authorId"
      icon="pi pi-pencil"
      rounded
      text
      @click="showEditDescriptionDialog()"
    />
  </div>

  <CardsList :authorId="deck.authorId" :type="CardType.Black" :cards="blackCards" />
  <CardsList :authorId="deck.authorId" :type="CardType.White" :cards="whiteCards" />

  <Dialog modal class="dialog" v-model:visible="isEditNameDialogVisible" header="Edit deck name" :closable="false">
    <InputText v-model="tempDeckName" class="edit-input" />
    <template #footer>
      <Button class="float-left" severity="secondary" label="Cancel" @click="hideEditNameDialog(false)" />
      <Button class="float-right" label="Save" @click="hideEditNameDialog(true)" />
    </template>
  </Dialog>

  <Dialog
    modal
    class="dialog"
    v-model:visible="isEditDescriptionDialogVisible"
    header="Edit deck description"
    :closable="false"
  >
    <Textarea v-model="tempDeckDescription" class="edit-input" />
    <template #footer>
      <Button class="float-left" severity="secondary" label="Cancel" @click="hideEditDescriptionDialog(false)" />
      <Button class="float-right" label="Save" @click="hideEditDescriptionDialog(true)" />
    </template>
  </Dialog>

  <Toast id="pending-changes-toast" position="bottom-right" group="save-toast">
    <template #message="slotProps">
      <i class="pi pi-exclamation-triangle p-toast-message-icon" />
      <div class="p-toast-message-text">
        <span class="p-toast-summary">{{ slotProps.message.summary }}</span>
        <div class="p-toast-detail">{{ slotProps.message.detail }}</div>
      </div>
      <Button label="Save" @click="updateDeck()" />
    </template>
  </Toast>
</template>

<script setup lang="ts">
import { computed, inject, reactive, ref, watch } from "vue";

import Button from "primevue/button";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";
import Toast from "primevue/toast";

import CardsList from "@/components/deck-management/CardsList.vue";
import { DeckManagementApiServiceKey } from "@/keys";
import { CardType } from "@/models/deck-management/api";
import DeckManagementApiService from "@/services/deck-management/apiService";
import { useUserStore } from "@/stores/user";
import { useToast } from "primevue/usetoast";

const { id } = defineProps<{
  id: number;
}>();

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
  throw new Error("DeckManagementApiService not initialized");
}

const toast = useToast();
const userStore = useUserStore();

const deck = reactive(await deckManagementApiService.getDeck(id));
const cards = reactive(await deckManagementApiService.getCards(id));

const whiteCards = computed(() => cards.filter(x => x.cardType === CardType.White));
const blackCards = computed(() => cards.filter(x => x.cardType === CardType.Black));

const isChanged = ref(false);

const isEditNameDialogVisible = ref(false);
const isEditDescriptionDialogVisible = ref(false);
const tempDeckName = ref(deck.name);
const tempDeckDescription = ref(deck.description);

const showEditNameDialog = () => {
  tempDeckName.value = deck.name;
  isEditNameDialogVisible.value = true;
};

const hideEditNameDialog = (save: boolean) => {
  if (save) {
    deck.name = tempDeckName.value.trim();
    isChanged.value = true;
  }
  isEditNameDialogVisible.value = false;
};

const showEditDescriptionDialog = () => {
  tempDeckDescription.value = deck.description;
  isEditDescriptionDialogVisible.value = true;
};

const hideEditDescriptionDialog = (save: boolean) => {
  if (save) {
    deck.description = tempDeckDescription.value.trim();
    isChanged.value = true;
  }
  isEditDescriptionDialogVisible.value = false;
};

const updateDeck = async () => {
  try {
    await deckManagementApiService.updateDeck(id, deck.description, deck.name);
    toast.add({
      summary: "Success",
      detail: "Deck updated",
      severity: "success",
      life: 3000,
    });
  } catch (error: unknown) {
    toast.add({
      summary: "An error has occurred",
      detail: error?.toString(),
      severity: "error",
      life: 3000,
    });
  }
  toast.removeGroup("save-toast");
};

watch(isChanged, () => {
  toast.add({
    summary: "Pending changes",
    detail: "You have unsaved changes.",
    group: "save-toast",
    severity: "warn",
    closable: false,
  });
});
</script>

<style lang="stylus">
.deck-name
  margin-bottom 0

.deck-description
  white-space pre

.deck-editor-edit-title
  cursor pointer
  border 1px solid black
  border-radius 50%
  padding 5px

.deck-editor-edit-description
  cursor pointer

.dialog
  width 50%

.edit-input
  width 100%
  margin-bottom 15px

#pending-changes-toast
  i.p-toast-message-icon
    font-size 18px
</style>
