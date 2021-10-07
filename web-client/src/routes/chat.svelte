<script>
    import { fade, fly } from "svelte/transition";
    import {
        apiGetLastMessageId,
        apiGetOlderMessagesFrom,
        apiGetSingleMessage,
    } from "../../scripts/api-helper";
    let username = "";
    let shown = false;
    export function startFetching(un) {
        console.log("fetching data user :", un);
        username = un;
        shown = true;
        messagesPromise = initial();
    }

    async function initial() {
        const latestMessage = await apiGetLastMessageId($authStore, username);
        //console.log("latest ok");
        const moreMessages = await more(latestMessage.messageId);
        //console.log("more ok", moreMessages);
        const messages = Promise.all(
            moreMessages.messageIdList.map(async (x) => {
                const msg = await apiGetSingleMessage($authStore, x);
                console.log(msg);
                return msg;
            })
        );
        //console.log("single ok", messages);
        return messages;
    }

    function more(messageId) {
        return apiGetOlderMessagesFrom($authStore, messageId);
    }

    let messagesPromise = Promise.resolve();

    import { createEventDispatcher, onMount } from "svelte";
    import { authStore } from "../store";

    const dispatch = createEventDispatcher();

    function goList() {
        dispatch("goList");
        shown = false;
    }
</script>

{#if shown}
    <div class="chat-box" transition:fly>
        <button on:click={goList}>back to list</button>
        <p>selam {username}</p>
        {#await messagesPromise}
            <p>loading</p>
        {:then data}
            <p>{JSON.stringify(data, null, 2)}</p>
        {/await}
    </div>
{/if}

<style>
    .chat-box {
        background-color: rgb(0 0 0 / 10%);
        text-align: center;
        padding: 1em;
        max-width: 240px;
        margin: 0 auto;
    }
</style>
