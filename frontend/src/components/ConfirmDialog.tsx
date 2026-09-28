import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material'

type Props = {
  open: boolean
  title: string
  message: string
  confirmLabel: string
  confirmColor?: 'error' | 'primary'
  onConfirm: () => void
  onClose: () => void
}

/** "Are you sure?" modal with No / Yes buttons. */
export function ConfirmDialog({ open, title, message, confirmLabel, confirmColor = 'error', onConfirm, onClose }: Props) {
  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle sx={{ fontFamily: "'Fraunces', Georgia, serif", fontWeight: 600 }}>{title}</DialogTitle>
      <DialogContent>
        <DialogContentText>{message}</DialogContentText>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2.5 }}>
        <Button onClick={onClose} color="inherit" variant="outlined">No</Button>
        <Button onClick={onConfirm} color={confirmColor} variant="contained">{confirmLabel}</Button>
      </DialogActions>
    </Dialog>
  )
}
