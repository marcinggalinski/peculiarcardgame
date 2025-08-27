<template>
  <div id="deck-editor">
    <h1 id="deck-name">
      {{ deck.name }}
      <Button
        v-if="userStore.id == deck.authorId"
        rounded
        text
        icon="pi pi-pencil"
        id="edit-name-button"
        @click="showEditNameDialog()"
      />
    </h1>
    <small id="deck-author">
      by {{ deck.author }}
      <span v-if="userStore.id == deck.authorId">(you)</span>
    </small>
    <div id="deck-description">
      {{ deck.description }}
      <Button
        v-if="userStore.id == deck.authorId"
        rounded
        text
        icon="pi pi-pencil"
        id="edit-description-button"
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

    <Dialog
      modal
      class="dialog"
      v-model:visible="isEditNameDialogVisible"
      header="Edit deck name"
      :closable="false"
      :draggable="false"
    >
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
      :draggable="false"
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
        <Button label="Discard" severity="secondary" @click="discardChanges()" />
        <Button label="Save" :loading="isUpdating" @click="updateDeck()" />
      </template>
    </Toast>
  </div>
</template>

<script setup lang="ts">
import { computed, inject, reactive, ref, watch } from "vue";

import Button from "primevue/button";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";
import Toast from "primevue/toast";
import { useToast } from "primevue/usetoast";

import CardsList from "@/components/deck-management/CardsList.vue";
import { DeckManagementApiServiceKey } from "@/keys";
import type { GetCardResponse, GetDeckResponse } from "@/models/deck-management/api";
import { CardType } from "@/models/deck-management/api";
import DeckManagementApiService from "@/services/deck-management/apiService";
import { useUserStore } from "@/stores/user";

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
  const cardsCopy = JSON.parse(JSON.stringify(data.cards)) as GetCardResponse[];
  const cards = cardsCopy.concat(updates.addedCards);
  for (const update of updates.cards) {
    cards.find(x => x.id === update.id)!.text = update.text;
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
    const trimmed = temp.value.trim();
    if (trimmed === data.deck.name) delete updates.name;
    else if (trimmed !== updates.name) updates.name = trimmed;
  }
  isEditNameDialogVisible.value = false;
};

const showEditDescriptionDialog = () => {
  temp.value = deck.value.description;
  isEditDescriptionDialogVisible.value = true;
};

const hideEditDescriptionDialog = (save: boolean) => {
  if (save) {
    const trimmed = temp.value.trim();
    if (trimmed === data.deck.description) delete updates.description;
    else if (trimmed !== updates.description) updates.description = trimmed;
  }
  isEditDescriptionDialogVisible.value = false;
};

const addCard = (text: string, cardType: CardType) => {
  updates.addedCards.push({ id: newCardId--, text, cardType });
};

const updateCard = (id: number, text: string) => {
  const trimmed = text.trim();

  const addedCard = updates.addedCards.find(x => x.id === id);
  if (addedCard && trimmed !== addedCard.text) {
    addedCard.text = trimmed;
    return;
  }

  const originalCard = data.cards.find(x => x.id === id)!;
  const updatedCard = updates.cards.find(x => x.id === id);
  if (updatedCard) {
    if (trimmed === originalCard.text) {
      const index = updates.cards.indexOf(updatedCard);
      updates.cards.splice(index, 1);
      return;
    }

    if (trimmed === updatedCard.text) return;

    updatedCard.text = trimmed;
    return;
  }

  if (trimmed === originalCard.text) return;

  updates.cards.push({ id, text: trimmed });
};

const deleteCard = (id: number) => {
  const index = updates.addedCards.findIndex(x => x.id === id);
  if (index > -1) {
    updates.addedCards.splice(index, 1);
    return;
  }

  updates.deletedCards.add(id);
};

const restoreCard = (id: number) => {
  updates.deletedCards.delete(id);
};

const discardChanges = () => {
  delete updates.name;
  delete updates.description;
  updates.cards = [];
  updates.addedCards = [];
  updates.deletedCards.clear();
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

<style scoped lang="stylus">
#deck-editor
  padding 10px

  #deck-name
    position relative
    margin-bottom 0
    padding-bottom 0
    width fit-content

    #edit-name-button
      position absolute
      top 0
      right 0

      background none
      transition 0.3s ease
      opacity 0

      height 39px
      width 39px

      transform translate(100%, 0)

  #deck-name:hover
    #edit-name-button
      transition 0.3s ease
      opacity 1

  #deck-author
    padding-left 25px

  #deck-description
    position relative
    margin-top 15px
    padding 5px
    white-space pre-wrap
    border 1px solid black
    border-radius 10px

    #edit-description-button
      position absolute
      top 0
      right 0

      background none
      transition 0.3s ease
      opacity 0

      height 39px
      width 39px

  #deck-description:hover
    #edit-description-button
      transition 0.3s ease
      opacity 1
</style>
<style lang="stylus">
.dialog
  width 50%

  .edit-input
    width 100%

  .p-inputtextarea.edit-input
    min-height 250px

#pending-changes-toast
  width fit-content
  min-width 25rem

  .p-button
    margin-left 12px

  i.p-toast-message-icon
    font-size 18px
</style>
