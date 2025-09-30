import * as cp from 'child_process';
import * as path from 'path';

export async function runGscCommand(command: string, workspaceRoot?: string): Promise<string> {
    const root = workspaceRoot ?? process.cwd();
    const script = path.join(root, '.specify', 'scripts', 'gsc', `gsc-${command}.ps1`);
    return new Promise((resolve) => {
        const pwsh = cp.spawn('pwsh', ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', script], {
            cwd: root,
            shell: false
        });

        let out = '';
        let err = '';
        pwsh.stdout.on('data', d => out += d.toString());
        pwsh.stderr.on('data', d => err += d.toString());
        pwsh.on('close', () => resolve(out || err));
    });
}
