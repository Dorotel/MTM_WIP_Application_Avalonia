export function formatCommandOutput(command: string, output: string): string {
    const ts = new Date().toISOString();
    return `# GSC: ${command}\n\nTime: ${ts}\n\n\u0060\u0060\u0060\n${output}\n\u0060\u0060\u0060\n`;
}
