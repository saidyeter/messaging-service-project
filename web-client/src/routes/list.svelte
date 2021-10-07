<script>
    import { each, onMount } from "svelte/internal";
    import { fade, fly } from "svelte/transition";
    import { authStore } from "../store";
    import { apiGetOpponents } from "../../scripts/api-helper";
    import Chat from "./chat.svelte";
    let chatbox;
    let contactList = [];
    onMount(() => {
        contactList = apiGetOpponents();
    });

    let shown = true;

    function goChat(event) {
        chatbox.startFetching(event.target.id);
        shown = false;
    }
    function goList(event) {
        shown = true;
    }
</script>

{#if shown}
    <div class="contact-list" trasition:fly>
        {#each contactList as contact}
            <div class="contact-item" id={contact.username} on:click={goChat}>
                {contact.displayname}
            </div>
        {/each}
    </div>
{/if}
<Chat bind:this={chatbox} on:goList={goList} />

<style>
    .contact-list {
        text-align: center;
        padding: 0em;
        max-width: 240px;
        margin: 0 auto;
    }
    .contact-item {
        width:auto;
        background-color: rgb(0 0 0 / 10%);
        padding: .5em;
        margin-top: .5em;
        margin-bottom: .5em;
        border-radius: 0.5em;
        text-align: left;
        cursor: pointer;
    }
</style>
