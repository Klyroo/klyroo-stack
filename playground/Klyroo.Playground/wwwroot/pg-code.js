// Scrolls the code box of the code panel so the first line the last knob changed (data-highlighted) is in view, without moving the page.
export function revealMarked(box) {
  const line = box?.querySelector('[data-slot="code-line"][data-highlighted="true"]');
  if (!line) return;
  const boxRect = box.getBoundingClientRect();
  const lineRect = line.getBoundingClientRect();
  const outside = lineRect.top < boxRect.top || lineRect.bottom > boxRect.bottom;
  if (outside) box.scrollTop += lineRect.top - boxRect.top - boxRect.height / 3;
}
