// HTML5 drag & drop for the Kanban board.
// Blazor Server cannot receive dragover/drop events directly, so this
// script attaches native listeners and forwards drops to C# via dotNet.
window.boardDnd = {
    init: (dotNet) => {
        document.querySelectorAll('.board-task-card[draggable="true"]').forEach((card) => {
            if (card.dataset.dndBound === "true") return;
            card.dataset.dndBound = "true";

            card.addEventListener('dragstart', (e) => {
                e.dataTransfer.setData('text/plain', card.dataset.taskId);
                e.dataTransfer.effectAllowed = 'move';
                card.classList.add('board-task-card-dragging');
            });

            card.addEventListener('dragend', () => {
                card.classList.remove('board-task-card-dragging');
            });
        });

        document.querySelectorAll('.board-column[data-status]').forEach((column) => {
            if (column.dataset.dndBound === "true") return;
            column.dataset.dndBound = "true";

            column.addEventListener('dragover', (e) => {
                e.preventDefault();
                e.dataTransfer.dropEffect = 'move';
                column.classList.add('board-column-drop-target');
            });

            column.addEventListener('dragleave', () => {
                column.classList.remove('board-column-drop-target');
            });

            column.addEventListener('drop', (e) => {
                e.preventDefault();
                column.classList.remove('board-column-drop-target');
                const taskId = e.dataTransfer.getData('text/plain');
                if (taskId) {
                    dotNet.invokeMethodAsync(
                        'OnTaskDropped',
                        taskId,
                        column.dataset.status);
                }
            });
        });
    }
};
