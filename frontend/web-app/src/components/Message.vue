<template>
  <div class="card-panel white-text red"
  	:class="{green : isSuccess, red: isError}">
      {{ text }}
    </div>
</template>

<script>
export default {
	props: ['text', 'type'],
	computed: {
		isSuccess() {
			return this.type === 'success'
		},
		isError() {
			return this.type === 'error'
		}
	}
}
</script>

<style>

</style>