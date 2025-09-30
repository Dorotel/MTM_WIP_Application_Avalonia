"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.formatCommandOutput = formatCommandOutput;
function formatCommandOutput(command, output) {
    const ts = new Date().toISOString();
    return `# GSC: ${command}\n\nTime: ${ts}\n\n\u0060\u0060\u0060\n${output}\n\u0060\u0060\u0060\n`;
}
