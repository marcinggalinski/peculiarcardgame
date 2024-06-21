<template>
  <h1 class="deck-name">
    {{ deck.name }}
    <Button
      rounded
      text
      v-if="userStore.id == deck.authorId"
      icon="pi pi-pencil"
      class="edit-name-button"
      @click="showEditNameDialog()"
    />
  </h1>
  <small class="deck-author">
    by {{ deck.author }}
    <span v-if="userStore.id == deck.authorId">(you)</span>
  </small>
  <div class="deck-description">
    {{ deck.description }}
    <Button
      rounded
      text
      v-if="userStore.id == deck.authorId"
      icon="pi pi-pencil"
      class="edit-description-button"
      @click="showEditDescriptionDialog()"
    />
  </div>

  <CardsList
    :authorId="deck.authorId"
    :type="CardType.Black"
    :cards="blackCards"
    :deleted-ids="updates.deletedCards"
    @add="(text, type) => addCard(text, type)"
    @update="(id, text) => updateCard(id, text)"
    @delete="id => deleteCard(id)"
    @restore="id => restoreCard(id)"
  />
  <CardsList
    :authorId="deck.authorId"
    :type="CardType.White"
    :cards="whiteCards"
    :deleted-ids="updates.deletedCards"
    @add="(text, type) => addCard(text, type)"
    @update="(id, text) => updateCard(id, text)"
    @delete="id => deleteCard(id)"
    @restore="id => restoreCard(id)"
  />

  <Dialog modal class="dialog" v-model:visible="isEditNameDialogVisible" header="Edit deck name" :closable="false">
    <InputText v-model="temp" class="edit-input" />
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
    <Textarea v-model="temp" class="edit-input" />
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
      <Button label="Save" :loading="isUpdating" @click="updateDeck()" />
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
import type { GetCardResponse, GetDeckResponse } from "@/models/deck-management/api";
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

const getData = async () => {
  const [deck, cards] = await Promise.all([
    deckManagementApiService.getDeck(id),
    deckManagementApiService.getCards(id),
  ]);

  data.deck = deck;
  data.cards = cards;
};

const toast = useToast();
const userStore = useUserStore();

const data = reactive<{ deck: GetDeckResponse; cards: GetCardResponse[] }>({ deck: null!, cards: [] });
const updates = reactive<{
  name?: string;
  description?: string;
  cards: { id: number; text: string }[];
  addedCards: GetCardResponse[];
  deletedCards: Set<number>;
}>({
  cards: [],
  addedCards: [],
  deletedCards: new Set<number>(),
});

const deck = computed<GetDeckResponse>(() => {
  return {
    id: data.deck.id,
    authorId: data.deck.authorId,
    author: data.deck.author,
    name: updates.name || data.deck.name,
    description: updates.description || data.deck.description,
    blackCardCount: blackCards.value.length,
    whiteCardCount: whiteCards.value.length,
  };
});
const cards = computed<GetCardResponse[]>(() => {
  const cards = data.cards.concat(updates.addedCards);
  for (const update of updates.cards) {
    cards.find(x => x.id == update.id)!.text = update.text;
  }
  return cards;
});

const whiteCards = computed(() => cards.value.filter(x => x.cardType === CardType.White));
const blackCards = computed(() => cards.value.filter(x => x.cardType === CardType.Black));

const isChanged = computed<boolean>(
  () =>
    Boolean(updates.name) ||
    Boolean(updates.description) ||
    updates.cards.length > 0 ||
    updates.addedCards.length > 0 ||
    updates.deletedCards.size > 0
);

const isEditNameDialogVisible = ref(false);
const isEditDescriptionDialogVisible = ref(false);
const temp = ref("");
const isUpdating = ref(false);
let newCardId = -1;

const showEditNameDialog = () => {
  temp.value = deck.value.name;
  isEditNameDialogVisible.value = true;
};

const hideEditNameDialog = (save: boolean) => {
  if (save) {
    updates.name = temp.value.trim();
  }
  isEditNameDialogVisible.value = false;
};

const showEditDescriptionDialog = () => {
  temp.value = deck.value.description;
  isEditDescriptionDialogVisible.value = true;
};

const hideEditDescriptionDialog = (save: boolean) => {
  if (save) {
    updates.description = temp.value.trim();
  }
  isEditDescriptionDialogVisible.value = false;
};

const addCard = (text: string, cardType: CardType) => {
  updates.addedCards.push({ id: newCardId--, text, cardType });
};

const updateCard = (id: number, text: string) => {
  let card = updates.cards.find(x => x.id == id);
  if (card) {
    card.text = text;
    return;
  }

  card = updates.addedCards.find(x => x.id == id);
  if (card) {
    card.text = text;
    return;
  }

  updates.cards.push({ id, text });
};

const deleteCard = (id: number) => {
  const index = updates.addedCards.findIndex(x => x.id == id);
  if (index > -1) {
    updates.addedCards.splice(index, 1);
    return;
  }

  updates.deletedCards.add(id);
};

const restoreCard = (id: number) => {
  updates.deletedCards.delete(id);
};

const updateDeck = async () => {
  isUpdating.value = true;

  try {
    await Promise.all([
      deckManagementApiService.updateDeck(id, updates.name, updates.description),
      ...updates.cards.map(x => deckManagementApiService.updateCard(x.id, x.text)),
      ...updates.addedCards.map(x => deckManagementApiService.addCard(deck.value.id, x.text, x.cardType)),
      ...Array.from(updates.deletedCards).map(x => deckManagementApiService.deleteCard(x)),
    ]);

    await getData();

    delete updates.name;
    delete updates.description;
    updates.cards = [];
    updates.addedCards = [];
    updates.deletedCards.clear();

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

  isUpdating.value = false;
};

watch(isChanged, () => {
  if (isChanged.value) {
    toast.add({
      summary: "Pending changes",
      detail: "You have unsaved changes.",
      group: "save-toast",
      severity: "warn",
      closable: false,
    });
  } else {
    toast.removeGroup("save-toast");
  }
});

await getData();
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

.edit-name-button
  height 39px
  width 39px

.edit-description-button
  height 39px
  width 39px

.dialog
  width 50%

  .edit-input
    width 100%

#pending-changes-toast
  i.p-toast-message-icon
    font-size 18px
</style>
