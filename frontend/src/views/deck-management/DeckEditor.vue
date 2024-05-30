<template>
    {{ deck }}
    {{ cards }}
</template>
<script setup lang="ts">
import { inject } from "vue";
import { DeckManagementApiServiceKey } from "@/keys";
import DeckManagementApiService from "@/services/deck-management/apiService";

const { id } = defineProps<{
    id: number
}>()

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
    throw new Error("DeckManagementApiService not initialized");
}

const deck = await deckManagementApiService.getDeck(id);
const cards = await deckManagementApiService.getCards(id);
</script>
<style lang="stylus"></style>