<template>
  <main class="deck-preview">
    <a class="deck-preview-name">{{ deck.name }}</a> by {{ deck.author }}
    <div class="deck-preview-description">{{ deck.description }}</div>
  </main>
</template>
<script setup lang="ts">
import type { GetDeckResponse } from '@/models/deck-management/api';

const props = defineProps<{
  deck: GetDeckResponse
}>();

</script>

<style scoped lang="stylus">
.deck-preview
  border 1px black solid
  border-radius 10px
  padding 15px
  margin 5px

  .deck-preview-name
    font-weight bold

</style>
