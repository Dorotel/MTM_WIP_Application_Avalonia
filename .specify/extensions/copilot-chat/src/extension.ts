import * as vscode from 'vscode';
import { registerGscChatHandlers } from './gsc-chat-handlers';

export function activate(context: vscode.ExtensionContext) {
    registerGscChatHandlers(context);
}

export function deactivate() {
    // no-op
}
