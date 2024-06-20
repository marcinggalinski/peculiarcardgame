<template>
  <div class="card-preview" :class="className">
    <div class="card-preview-text">{{ card.text.replaceAll("_", "_____") }}</div>
    <div v-if="card.cardType === CardType.Black" class="card-preview-picks">Pick {{ picks }}</div>
  </div>
</template>

<script setup lang="ts">
import type { GetCardResponse } from "@/models/deck-management/api";
import { CardType } from "@/models/deck-management/api";

const { card } = defineProps<{
  card: GetCardResponse;
}>();

const className = card.cardType === CardType.Black ? "black" : "white";
const picks = card.cardType === CardType.Black ? card.text.split("_").length - 1 || 1 : 0;
</script>

<style lang="stylus">
.card-preview
  display flex
  flex-direction column

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

.black
  background black
  color white

.white
  background white
  color black
</style>
