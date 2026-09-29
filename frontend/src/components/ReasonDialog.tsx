import { useState } from 'react'
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Stack, TextField } from '@mui/material'

type Props = {
  title: string
  message: string
  label: string
  helperText: string
  required?: boolean
  confirmLabel: string
  onConfirm: (reason: string) => void
  onClose: () => void
}

/** "Are you sure?" with a reason box. Required reasons keep Yes disabled until filled. */
export function ReasonDialog({ title, message, label, helperText, required = false, confirmLabel, onConfirm, onClose }: Props) {
  const [reason, setReason] = useState('')

  return (
    <Dialog open onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle sx={{ fontFamily: "'Fraunces', Georgia, serif", fontWeight: 600 }}>{title}</DialogTitle>
      <DialogContent>
        <Stack spacing={2.5}>
          <DialogContentText>{message}</DialogContentText>
          <TextField
            label={label}
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            helperText={helperText}
            slotProps={{ htmlInput: { maxLength: 200 } }}
            multiline
            minRows={2}
            required={required}
            autoFocus
          />
        </Stack>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2.5 }}>
        <Button onClick={onClose} color="inherit" variant="outlined">No</Button>
        <Button onClick={() => onConfirm(reason.trim())} color="error" variant="contained" disabled={required && !reason.trim()}>
          {confirmLabel}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
