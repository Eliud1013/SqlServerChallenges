let editor = null;

require.config({
    paths: {'vs': 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs'}
});

require(['vs/editor/editor.main'], function () {
    monaco.editor.defineTheme('default-theme', {
        base: 'vs-dark',
        inherit: true,
        rules: [
            {token: 'keyword', foreground: '569CD6', fontStyle: 'bold'},
            {token: 'string', foreground: 'CE9178'},
            {token: 'comment', foreground: '6A9955'}
        ],
        colors: {
            'editor.background': '#0D0F1B',
            'editor.lineHighlightBackground': '#222222',
            'editorCursor.foreground': '#00E676'
        }
    });

    editor = monaco.editor.create(document.getElementById('challenge-detail-editor'), {
        value: "SELECT\n    BusinessEntityID,\n    FirstName,\n    LastName\nFROM Person.Person\nWHERE PersonType = 'EM';",
        language: 'sql',
        theme: 'default-theme',
        fontFamily: 'Fira Code, Consolas, monospace',
        automaticLayout: true,
        fontSize: 14,
        minimap: {enabled: false},
        scrollBeyondLastLine: false,
        lineNumbers: 'on',
        roundedSelection: false,
        tabSize: 4
    });

    Split(['#challenge-detail-left', '#challenge-detail-right'], {
        sizes: [40, 60],
        minSize: [350, 400],
        gutterSize: 6,
        direction: 'horizontal',
        cursor: 'col-resize'
    });

    Split(['#challenge-detail-editor', '#challenge-detail-results'], {
        sizes: [60, 40],
        minSize: [160, 100],
        gutterSize: 6,
        direction: 'vertical',
        cursor: 'row-resize'
    });

    editor.layout();
});

export function getEditorValue() {
    return editor ? editor.getValue() : '';
}

export function setSyntaxErrors(errors) {
    if (!editor || !editor.getModel()) return;
    const markers = errors.map(function (e) {
        return {
            severity: monaco.MarkerSeverity.Error,
            message: e.message,
            startLineNumber: e.line,
            startColumn: 1,
            endLineNumber: e.line,
            endColumn: Number.MAX_SAFE_INTEGER
        };
    });
    monaco.editor.setModelMarkers(editor.getModel(), 'owner', markers);
}

export function clearSyntaxErrors() {
    if (!editor || !editor.getModel()) return;
    monaco.editor.setModelMarkers(editor.getModel(), 'owner', []);
}