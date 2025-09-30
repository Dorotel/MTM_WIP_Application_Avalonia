"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = activate;
exports.deactivate = deactivate;
const gsc_chat_handlers_1 = require("./gsc-chat-handlers");
function activate(context) {
    (0, gsc_chat_handlers_1.registerGscChatHandlers)(context);
}
function deactivate() {
    // no-op
}
