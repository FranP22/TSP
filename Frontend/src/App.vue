<script setup lang="ts">
  import { RouterView } from 'vue-router';
  import NavigationView from './components/NavigationView.vue';
</script>

<template>
  <header>
    <NavigationView/>
  </header>

  <main class="flex min-h-[calc(100vh-73px)] items-center justify-center bg-slate-50 px-4 py-12">
    <RouterView/>
  </main>
</template>