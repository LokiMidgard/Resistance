package de.sequence2tiles;

import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;

public class GUI {
	private static JTextField source,
		destination;
	
	public static void main(String[] args) {
		JFrame frame = new JFrame("Sequence2Tiles");
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		
		frame.setLayout(new GridBagLayout());
		
		GridBagConstraints c = new GridBagConstraints();
		c.fill = GridBagConstraints.BOTH;
		c.gridx = 0;
		c.gridy = 0;
		
		frame.add(new JPanel(),c);
		c.gridy++;
		c.gridx++;
		frame.add(new JLabel("Source Folder: "), c);
		c.gridx++;
		frame.add(new JPanel(),c);
		c.gridx++;
		source = new JTextField(Sequence2Tiles.getSourceDirectory(),20);
		frame.add(source, c);
		c.gridx++;
		frame.add(new JPanel(),c);
		c.gridx++;
		JButton browseSourceButton = new JButton("Browse");
		browseSourceButton.setEnabled(false);
		frame.add(browseSourceButton, c);
		
		c.gridx = 1;
		c.gridy++;
		frame.add(new JPanel(),c);
		c.gridy++;
		
		frame.add(new JLabel("Destination Folder: "), c);
		c.gridx++;
		frame.add(new JPanel(),c);
		c.gridx++;
		destination = new JTextField(Sequence2Tiles.getSourceDirectory(),20);
		frame.add(destination, c);
		
		c.gridx++;
		frame.add(new JPanel(),c);
		c.gridx++;
		JButton browseDestinationButton = new JButton("Browse");
		browseDestinationButton.setEnabled(false);
		frame.add(browseDestinationButton, c);
		
		c.gridy++;
		frame.add(new JPanel(),c);
		c.gridy ++;
		
		c.gridx = 1;
		frame.add(new JLabel("Result Name: "),c);
		c.gridx +=2;
		JTextField tf = new JTextField();
		frame.add(tf, c);
		tf.setEnabled(false);
//		c.gridy ++;
		c.gridx += 2;
		JButton submit = new JButton("Start");
		frame.add(submit, c);
		c.gridx++;
		c.gridy++;
		frame.add(new JPanel(),c);
		
		submit.addActionListener(new ActionListener() {
			
			@Override
			public void actionPerformed(ActionEvent e) {
				Sequence2Tiles.run(source.getText(), destination.getText());				
			}
		});
		
		frame.pack();
		frame.setVisible(true);
		frame.setLocationRelativeTo(null);
	}

}
