<template>
  <RouterLink :to="{ name: 'deck-editor', params: { id: deck.id } }" v-if="deck" class="deck-preview clear">
    <!-- <div v-if="deck" class="deck-preview"> -->
    <div class="deck-preview-header">
      <span class="deck-preview-name">{{ deck.name }}</span>
      <!-- <br> -->
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
    <!-- </div> -->
  </RouterLink>
</template>

<script setup lang="ts">
import type { GetDeckResponse } from "@/models/deck-management/api";

defineProps<{
  deck: GetDeckResponse;
}>();
</script>

<style scoped lang="stylus">
.deck-preview
  flex 1 0 20%
  max-width 250px
  max-height 350px
  aspect-ratio 5 / 7

  display flex
  flex-direction column

  border 1px black solid
  border-radius 15px

  margin 5px

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

    margin -1px
    padding 15px

    .deck-preview-description
      font-style italic

  .deck-preview-footer
    border-radius 0 0 15px 15px

    margin auto -1px -1px -1px
    padding 15px

    background black
    color white
</style>
