import * as vscode from 'vscode';
import { formatCommandOutput } from './chat-formatters';
import { runGscCommand } from './state-bridge';

// Canonical script-backed commands (PowerShell files exist for these)
const canonicals = [
    'constitution', 'specify', 'clarify', 'plan', 'task', 'analyze', 'implement', 'memory', 'validate', 'status', 'rollback', 'help', 'update'
] as const;

// Easy-to-read aliases mapped to canonical command names
// Note: Only map to existing PowerShell scripts to avoid missing-command errors
const aliasMap: Record<string, string> = {
    // constitution
    'check-rules': 'constitution',
    'rules-check': 'constitution',
    'check-constitution': 'constitution',

    // specify
    'write-spec': 'specify',
    'make-spec': 'specify',
    'new-spec': 'specify',

    // clarify
    'clarify-questions': 'clarify',
    'ask-questions': 'clarify',
    'clarify-needs': 'clarify',

    // plan
    'make-plan': 'plan',
    'create-plan': 'plan',

    // task
    'make-tasks': 'task',
    'create-tasks': 'task',
    'generate-tasks': 'task',

    // analyze
    'analyze-work': 'analyze',
    'review-work': 'analyze',

    // implement
    'do-work': 'implement',
    'run-implementation': 'implement',
    'apply-changes': 'implement',

    // memory
    'show-memory': 'memory',
    'show-knowledge': 'memory',

    // validate
    'check-work': 'validate',
    'check-workflow': 'validate',
    'validate-work': 'validate',

    // status
    'show-status': 'status',
    'show-progress': 'status',

    // rollback
    'go-back': 'rollback',
    'reset-step': 'rollback',

    // help
    'show-help': 'help',

    // update
    'update-spec': 'update',
    'edit-spec': 'update',
    'change-spec': 'update',
    'modify-spec': 'update',
    'spec-update': 'update'
};

export function registerGscChatHandlers(context: vscode.ExtensionContext) {
    const all = [...canonicals, ...Object.keys(aliasMap)];
    for (const c of all) {
        const cmdId = `mtmGsc.${c}`;
        const disposable = vscode.commands.registerCommand(cmdId, async () => {
            const canonical = (aliasMap[c] ?? c) as typeof canonicals[number];
            const root = vscode.workspace.workspaceFolders?.[0]?.uri.fsPath ?? process.cwd();
            const result = await runGscCommand(canonical, root);
            const doc = await vscode.workspace.openTextDocument({
                content: formatCommandOutput(c, result),
                language: 'markdown'
            });
            await vscode.window.showTextDocument(doc, { preview: true });
        });
        context.subscriptions.push(disposable);
    }
}
